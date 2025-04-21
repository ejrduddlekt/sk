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
    [SerializeField] private TMP_Text statusText;       // 연결 상태 표시
    [SerializeField] private TMP_Text displayText;      // 수신 메시지 표시
    [SerializeField] private TMP_InputField inputField; // 전송할 텍스트 입력
    [SerializeField] private Button sendButton;         // 전송 버튼
    [SerializeField] private Button connectButton;      // 연결 버튼

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
        connectButton.onClick.AddListener(OnConnectButton);
        sendButton.onClick.AddListener(OnSendButton);

        // 초기 UI 세팅
        statusText.text = _statusMessage;
        sendButton.interactable = false;
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

            // 읽기 루프: WinForms → Unity
            while (_running && _pipe.IsConnected)
            {
                string line = _reader.ReadLine();
                if (line != null)
                    _incoming.Enqueue(line);
                else
                    Thread.Sleep(5);
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
            sendButton.interactable = true;
            _connectedFlag = false;
        }

        // 2) 상태 메시지 UI 업데이트
        if (statusText.text != _statusMessage)
            statusText.text = _statusMessage;

        // 3) 큐에 쌓인 수신 메시지를 displayText에 추가
        while (_incoming.TryDequeue(out var msg))
        {
            displayText.text += msg + "\n";


            // Json 파씽 예시
            WaferList parsedData = JsonUtility.FromJson<WaferList>(msg);
            foreach (var wafer in parsedData.waferList)
            {
                Debug.Log($"LOT_ID: {wafer.LOT_ID}, WF_ID: {wafer.WF_ID}, PRODUCT_STACK: {wafer.PRODUCT_STACK}, POSITION: {wafer.POSITION}");
            }
        }
    }

    private void OnSendButton()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text) || _writer == null || !_pipe.IsConnected)
            return;

        _writer.WriteLine(text);
        inputField.text = "";

        _writer.Flush();
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
