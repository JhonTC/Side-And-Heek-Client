using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WindVolume : MonoBehaviour
{
    public static WindVolume Instance;

    public static Dictionary<ushort, WindReciever> WindRecievers = new Dictionary<ushort, WindReciever>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private float windSpeed;

    [SerializeField, Range(0, 10)]
    private float speedVariance;

    [SerializeField]
    private Vector3 directionVariance;

    [SerializeField]
    private NetworkPhysicsBody[] networkPhysicsBodies;

    private ushort currentRecieverId = 0;

    public void Init()
    {
        foreach (NetworkPhysicsBody networkPhysicsBody in networkPhysicsBodies)
        {
            if (NetworkManager.NetworkType == NetworkType.Client)
            {
                networkPhysicsBody.DisablePhysics();
            }

            WindRecievers.Add(currentRecieverId, new WindReciever(networkPhysicsBody, networkPhysicsBody.transform.localScale.y));
            currentRecieverId++;
        }
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.IsConnected) return;

        if (NetworkManager.NetworkType != NetworkType.Client)
        {
            foreach (var windRecieverDict in WindRecievers)
            {
                WindReciever windReciever = windRecieverDict.Value;

                Vector3 direction = transform.forward + new Vector3(Random.Range(-directionVariance.x, directionVariance.x), 0, Random.Range(-directionVariance.z, directionVariance.z));
                Vector3 force = direction * windSpeed * Random.Range(0, speedVariance) * (2 - windReciever.scale);
                windReciever.networkPhysicsBody.root.AddForce(force);

                ServerSend.WeatherObjectTransform(windRecieverDict.Key, windReciever.networkPhysicsBody);
            }
        }
    }

    [System.Serializable]
    public class WindReciever
    {
        public NetworkPhysicsBody networkPhysicsBody;
        public float scale;

        public WindReciever(NetworkPhysicsBody networkPhysicsBody, float scale)
        {
            this.networkPhysicsBody = networkPhysicsBody;
            this.scale = scale;
        }
    }
}
