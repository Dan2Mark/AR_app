using UnityEngine;

public class BoatPositionAdjustmentWithCompass : MonoBehaviour
{
    public float distanceSouthWest = 10.0f;  // �������� �� ���-����� � ������
    public float dropHeight = 2.0f;          // �������� ���� � ������

    void Start()
    {
        // ��������, ��� ������ �������
        Input.compass.enabled = true;

        // �������� ����� �� 10 ������ �� ���-����� � ������ ���������� ����������
        ShiftPositionSouthWestWithCompass();

        // �������� ����� �� 2 ����� ����
        LowerBoat();
    }

    void ShiftPositionSouthWestWithCompass()
    {
        // ���-�������� ����������� � ��� 225 �������� ������������ ������
        float targetAngleSW = 225.0f;

        // �������� ������� ����������� ���������� (������ ��������� �� �����)
        float currentHeading = Input.compass.trueHeading;

        // ������������ ���� ���������� ����� ������� � ���-�������
        float adjustedAngle = targetAngleSW - currentHeading;

        // ����������� ���� ���� � ������ �����������
        Vector3 directionSouthWest = Quaternion.Euler(0, adjustedAngle, 0) * Vector3.forward;

        // ��������� ���������� ������ ������������ ����
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;  // ������� ������ �� ��� Y (������), ����� �������� ������ �� ���������
        cameraForward.Normalize();

        // ������� ������� ����� �� �������� ���������� � ���� �����������
        transform.position += directionSouthWest * distanceSouthWest;
    }

    void LowerBoat()
    {
        // �������� ����� �� dropHeight ����
        transform.position = new Vector3(transform.position.x, transform.position.y - dropHeight, transform.position.z);
    }

    void OnDestroy()
    {
        // ��������� ������, ����� ����� ����������
        Input.compass.enabled = false;
    }
}
