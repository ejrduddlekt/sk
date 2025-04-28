using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Data;

public class NamedPipeManualClient : MonoBehaviour
{
    [Header("Pipe Settings")]
    [SerializeField] private string pipeName = "MyPipe";

    [Header("UI References")]
    [SerializeField] private TMP_Text displayText;      // 수신 메시지 표시

    [Header("Item Manager")]
    [Tooltip("Wafer 생성 요청을 보낼 ItemManager 할당")]
    [SerializeField] private ItemManager itemManager;

    // 내부 상태 및 큐
    private NamedPipeClientStream _pipe;
    private StreamReader _reader;
    private StreamWriter _writer;
    private Thread _ioThread;
    private bool _running;
    private bool _connectedFlag;
    private string _statusMessage = "Disconnected";
    private ConcurrentQueue<string> _incoming = new ConcurrentQueue<string>();

    void Awake()
    {
        // 버튼 이벤트
        OnConnectButton();
    }

    void OnConnectButton()
    {
        if (_running) return;

        _running = true;
        _statusMessage = "Connecting...";
        _ioThread = new Thread(IOWorker) { IsBackground = true };
        _ioThread.Start();
    }

    private void IOWorker()
    {
        try
        {
            Debug.Log($"[Pipe] Trying to connect to '{pipeName}'...");
            _pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            _pipe.Connect(5000);  // 5초 타임아웃

            // 스트림 초기화
            _reader = new StreamReader(_pipe, Encoding.UTF8);
            _writer = new StreamWriter(_pipe, Encoding.UTF8);

            // 메인 스레드에서 처리하도록 플래그 설정
            _connectedFlag = true;

            Debug.Log("[Pipe] Connected!");

            StringBuilder jsonBuilder = new StringBuilder();

            while (_running && _pipe.IsConnected)
            {
                string? line = _reader.ReadLine();
                if (line != null)
                {
                    if (line.Trim() == "<END>")
                    {
                        _incoming.Enqueue(jsonBuilder.ToString());
                        jsonBuilder.Clear();
                    }
                    else
                    {
                        jsonBuilder.AppendLine(line);
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Pipe] Exception: {ex.GetType().Name} – {ex.Message}");
            _statusMessage = $"Error: {ex.Message}";
        }
    }

    void Update()
    {
        // 1) 연결 성공 플래그가 세워지면 UI 갱신
        if (_connectedFlag)
        {
            _statusMessage = "Connected";
            _connectedFlag = false;
        }

        Recv();
    }

    private void Recv()
    {
        while (_incoming.TryDequeue(out var msg))
        {
            displayText.text = msg;

            if (msg.Contains("wafer_list"))
            {
                WaferList parsedData = JsonUtility.FromJson<WaferList>(msg);
                displayText.text = $"파싱 성공: {parsedData.wafer_list.Count}개";

                itemManager.SpawnWafers(parsedData.wafer_list);
            }

            if (msg.Contains("stackmap_list"))
            {
                StackMapList parsedData = JsonUtility.FromJson<StackMapList>(msg);
                displayText.text = $"파싱 성공: {parsedData.stackmap_list.Count}개";

                // 덕영쓰 여기서부터 작업하면됨!
            }

            if (msg.Contains("noinkmap_list"))
            {
                NoInkMapList parsedData = JsonUtility.FromJson<NoInkMapList>(msg);
                displayText.text = $"파싱 성공: {parsedData.noinkmap_list.Count}개";

                // 덕영쓰 여기서부터 작업하면됨!
            }

            //TODO : 차후 넘어오는 데이터를 분기처리로 받아서 파씽하면됨
        }
    }

    public void Send(string message)
    {
        if (_writer == null || !_pipe.IsConnected) return;

        try
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Pipe] Send failed: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        // 안전하게 종료
        _running = false;
        if (_ioThread != null && _ioThread.IsAlive)
            _ioThread.Join(100);

        _reader?.Close();
        _writer?.Close();
        _pipe?.Close();
    }
}
