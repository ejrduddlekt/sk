using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;


public class Generate : MonoBehaviour
{
    // ── Public 변수들 ────────────────────────────────
    // 그리드의 가로(열) 및 세로(행) 칸 수를 결정합니다.
    public int horizontal = 10;
    public int vertical = 10;
    // 칩(오브젝트) 생성 시 성공률. 0.8이면 80% 확률로 첫 번째 프리팹을 사용.
    public float rateSuccess = 0.8f;
    // 칩들을 배치할 때 원의 내부에 배치할지 결정하는 원의 반지름.
    public float circleRadius = 4.0f;
    // 각 칩의 가로 및 세로 크기
    public float chipWidth = 1.0f;
    public float chipHeight = 1.0f;
    // 칩들 사이의 간격(여백)
    public float chipPadding = 0.1f;
    // 칩으로 사용할 프리팹 리스트. index 0: 성공, index 1: 실패 등으로 사용.
    public List<GameObject> prefabChips;
    // 생성된 맵의 문자열 정보를 출력할 TextMeshProUGUI
    public TextMeshProUGUI textMap;

    // ── Private 변수 ────────────────────────────────
    // 맵 정보(칩 배치 상태)를 문자로 저장할 문자열 변수.
    private string map = "";

    // Start는 스크립트가 활성화될 때 최초 한 번 호출됩니다.
    void Start()
    {
        // horizontal과 vertical 중 더 큰 값 (현재 사용되지는 않지만 이후에 활용할 수도 있음)
        int maximum = horizontal > vertical ? horizontal : vertical;

        // 프리팹 리스트에 하나 이상의 프리팹이 존재해야 실행
        if (prefabChips.Count > 0)
        {
            float x, y;

            // 전체 Y 길이 계산: (칩 높이 * 행 수) + (행 사이 간격)
            float totalY = vertical * chipHeight;
            if (vertical > 0)
            {
                totalY += (vertical - 1) * chipPadding;
            }
            // 전체 X 길이 계산: (칩 너비 * 열 수) + (열 사이 간격)
            float totalX = horizontal * chipWidth;
            if (horizontal > 0)
            {
                totalX += (horizontal - 1) * chipPadding;
            }

            // 맵 문자열 초기화 (기존 내용 삭제)
            map = "";

            // Y 좌표 초기값 설정 (중앙 정렬을 위해 전체 높이의 절반에서 칩 높이만큼 뺌)
            y = (totalY - chipHeight) / 2.0f;
            // 행(세로) 루프
            for (int i = 0; i < vertical; i++)
            {
                // 각 행의 시작 X 좌표 설정 (중앙 정렬을 위해 전체 너비의 절반에서 칩 너비만큼 빼줌)
                x = -(totalX - chipWidth) / 2.0f;
                // 열(가로) 루프
                for (int j = 0; j < horizontal; j++)
                {
                    // 현재 위치 (x, y)가 원 내부에 있는지 확인 (원점에서의 거리 계산)
                    if (x * x + y * y <= circleRadius * circleRadius)
                    {
                        // 랜덤값으로 성공 여부 결정.
                        // rateSuccess 미만이면 index 0 (예: 성공), 그렇지 않으면 index 1 (예: 실패)
                        int index = Random.Range(0f, 1.0f) < rateSuccess ? 0 : 1;
                        
                        // 월드 기준 위치 = 부모 위치 + (x,y,0)
                        Vector3 spawnPos = transform.position + new Vector3(x, y, 0);

                        // Instantiate 에 부모까지 한 번에 지정
                        GameObject chip = Instantiate(
                            prefabChips[index],
                            spawnPos,
                            Quaternion.identity,
                            transform      // worldPositionStays=true (기본)
                        );

                        // 스케일 설정
                        chip.transform.localScale = new Vector3(chipWidth, chipHeight, 1);

                        // 생성된 칩을 현재 오브젝트(Generate 스크립트가 붙은 오브젝트)의 자식으로 설정.
                        chip.transform.parent = transform;
                        // 칩의 크기를 지정한 width, height 값으로 설정.
                        chip.transform.localScale = new Vector3(chipWidth, chipHeight, 1);
                        // 생성된 칩 타입에 따라 맵 문자열에 'O' 또는 'X' 추가.
                        if (index == 0)
                        {
                            map += 'O';
                        }
                        else
                        {
                            map += 'X';
                        }
                    }
                    else
                    {
                        // 원의 영역 밖이면 '.' 문자 추가.
                        map += '.';
                    }

                    // 다음 칩을 배치하기 위해 X 좌표 이동(칩 너비 + 칩 사이 여백)
                    x += chipWidth + chipPadding;
                }
                // 한 행이 끝나면 개행 문자 추가하여 다음 줄로 이동.
                map += '\n';
                // 다음 행을 배치하기 위해 Y 좌표를 내림(칩 높이 + 칩 사이 여백)
                y -= chipHeight + chipPadding;
            }

            // textMap이 할당되어 있다면 최종 맵 문자열과 MD5 해시값을 출력.
            if (textMap != null)
            {
                // 생성된 맵 문자열 뒤에 MD5 해시값을 붙임.
                map += "MD5: " + ComputeMD5(map);
                // UI 텍스트에 맵 문자열 출력
                textMap.text = map;
                // 텍스트 UI의 크기를 그리드 크기에 맞게 조정 (임의의 배율 사용: horizontal*5.0, vertical*12.0)
                textMap.rectTransform.sizeDelta = new Vector2(horizontal * 5.0f, vertical * 12.0f);
            }
        }
    }

    // ── MD5 해시값 계산 함수 ────────────────────────────────
    // 입력된 문자열의 MD5 해시값을 계산하여 16진수 문자열로 반환
    private string ComputeMD5(string input)
    {
        // MD5 객체 생성 (using 구문 사용으로 자원 자동 해제)
        using (MD5 md5 = MD5.Create())
        {
            // 입력 문자열을 UTF8 바이트 배열로 변환
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            // MD5 해시 계산
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // 해시값을 16진수 문자열로 변환 (대문자 형식, 예: "A3B2C...")
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2")); // "X2" 형식은 두 자리의 대문자 16진수를 만듭니다.
            }
            return sb.ToString();
        }
    }

    // ── Update 함수 ────────────────────────────────
    // 매 프레임 호출되지만, 이 예제에서는 사용하지 않습니다.
    void Update()
    {
        
    }
}
