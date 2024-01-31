using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork1 : NetworkBehaviour
{
    private readonly NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 vel;
    private float rotVel;
    [SerializeField] private float cheapInterpolationTime = 0.1f;
    private void Update()
    {
        if(IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };
        }
        else
        {
            transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, netState.Value.Position, ref vel, cheapInterpolationTime), Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, netState.Value.Rotation.y, ref rotVel, cheapInterpolationTime), 0));
        }
    }

    struct PlayerNetworkData : INetworkSerializable
    {
        private float x, z;
        private short yRotation;

        internal Vector3 Position
        {
            get => new(x, 0, z);
            set
            {
                x = value.x;
                z = value.z;
            }
        }

        internal Vector3 Rotation
        {
            get => new(0, yRotation, 0);
            set => yRotation = (short)value.y;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref z);

            serializer.SerializeValue(ref yRotation);
        }
    }
}
