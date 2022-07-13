using UnityEngine;

//[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget, cameraTransform, targetGround;
    [SerializeField] private Vector3 targetOffset, zoomMaxClamp, zoomMinClamp;
    [SerializeField, Range(0.1f, 10f)] private float cameraSmoothFactor = 2.5f;
    [SerializeField, Range(1f, 20f)] private float zoomSpeed = 5f, zoomPower = 15f;

    private Camera cameraComponent;
    private Shakeable shakeableComponent;

    private Vector3 nextPosition, nextZoom, groundSize;
    private Vector3 dragStartPosition, dragCurrentPosition;

    private void Start()
    {
        cameraComponent = cameraTransform.GetComponent<Camera>();
        shakeableComponent = GetComponent<Shakeable>();
        groundSize = targetGround.GetComponent<Terrain>().terrainData.size;

        LerpToTarget();
        nextZoom = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        transform.position = Vector3.Lerp(position, nextPosition, cameraSmoothFactor * Time.deltaTime);
        if (shakeableComponent != null)
        {
            shakeableComponent.UpdatePosition(transform.position);
        }
        HandleZoom();
    }

    public void LerpToTarget()
    {
        nextPosition = followTarget.position + targetOffset;
        BindCamera();
    }

    private void BindCamera()
    {
        if (nextPosition.x >= groundSize.x * 0.65f || nextPosition.x <= -groundSize.x * 0.65f)
            nextPosition.x = transform.position.x * 0.95f;
        if (nextPosition.z >= groundSize.z * 0.65f || nextPosition.z <= -groundSize.z * 0.65f)
            nextPosition.z = transform.position.z * 0.95f;
    }

    private void HandleZoom()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0f)
        {
            nextZoom += cameraTransform.forward * mouseScroll * zoomPower;
            if (nextZoom.x > zoomMaxClamp.x || nextZoom.y > zoomMaxClamp.y || nextZoom.z < zoomMaxClamp.z)
                nextZoom = zoomMaxClamp;
            else if (nextZoom.x < zoomMinClamp.x || nextZoom.y < zoomMinClamp.y || nextZoom.z > zoomMinClamp.z)
                nextZoom = zoomMinClamp;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, nextZoom, Time.deltaTime * zoomSpeed);
    }

    public void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
                dragStartPosition = ray.GetPoint(entry);
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                nextPosition = transform.position + dragStartPosition - dragCurrentPosition;
                BindCamera();
            }
        }
    }
}
