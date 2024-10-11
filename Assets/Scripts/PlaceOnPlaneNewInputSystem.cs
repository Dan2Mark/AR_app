using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnExtendedPlane : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;
    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector3 lastPlanePosition;
    private bool isPlaneFound = false;

    private TouchControls controls;
    private bool isPressed;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();
        controls.control.touch.performed += _ => isPressed = true;
        controls.control.touch.canceled += _ => isPressed = false;
    }

    private void Update()
    {
        if (Pointer.current == null || !isPressed)
            return;

        var touchPosition = Pointer.current.position.ReadValue();

        // ���� �������� ��������� �� ������
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            lastPlanePosition = hitPose.position; // ���������� ������� ��������� �� ������
            isPlaneFound = true;

            PlaceObject(hitPose);
        }
        else if (isPlaneFound)
        {
            // ���� ��������� ������� �� ������, ��������� � �� ����
            Vector3 extendedPosition = ExtendPlaneOverWater(touchPosition);
            PlaceObject(new Pose(extendedPosition, Quaternion.identity));
        }
    }

    private Vector3 ExtendPlaneOverWater(Vector2 touchPosition)
    {
        // ����������� ���������� ������� � ���
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        Vector3 waterPlaneNormal = Vector3.up; // ������ ������� ��������� ����

        // ��������� ����� ������� �� ����������� � ����
        Plane waterPlane = new Plane(waterPlaneNormal, lastPlanePosition);
        waterPlane.Raycast(ray, out float distanceToWater);

        // ���������� ������� �� ����
        return ray.GetPoint(distanceToWater);
    }

    private void PlaceObject(Pose pose)
    {
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(placedPrefab, pose.position, pose.rotation);
        }
        else
        {
            spawnedObject.transform.position = pose.position;
            spawnedObject.transform.rotation = pose.rotation;
        }

        // ������ ������ ����� �������� �� ������
        Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
        lookPos.y = 0;
        spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void OnEnable()
    {
        controls.control.Enable();
    }

    private void OnDisable()
    {
        controls.control.Disable();
    }
}
