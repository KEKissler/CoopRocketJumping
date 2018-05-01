using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class projectileController : NetworkBehaviour
{

    private Vector2 direction;
    public float speed;
    public float minColliderSize;
    public float maxColliderSize;
    public float minExplosionSize;
    public float maxExplosionSize;
    public float colliderSizeDelta;
    public float explosionSizeDelta;
    public float minForce;
    public float maxForce;
    public float forceDelta;
    private float currentForce;
    [SyncVar(hook = "OnSetColl")]
    private float currentColliderSize;
    [SyncVar(hook = "OnSetVelo")]
    [HideInInspector]
    public Vector2 velo;
    private float currentExplosionSize;
    private bool fired = false;
    public GameObject explosion;
    [HideInInspector]
    public Transform playerWhoFiredThis;
    [HideInInspector]
    public NetworkInstanceId netIdOfWhoFiredThis;
    public float timeToLive;
    private float timeAlive;

    // Use this for initialization
    void Start()
    {
        timeAlive = 0f;
    }

    private void OnSetColl(float newCollSize)
    {
        currentColliderSize = newCollSize;
        transform.localScale = new Vector3(newCollSize, newCollSize, 1);
    }

    private void OnSetVelo(Vector2 newVector)
    {
        velo = newVector;
        GetComponent<Rigidbody2D>().velocity = newVector;
    }

    // Update is called once per frame
    void Update()
    {
        //managing timeToLive and self destruction, only on server since anything destroyed there is propogated to all clients
        if (isServer)
        {
            if (timeAlive > timeToLive)
            {
                Destroy(this.gameObject);
            }
            timeAlive += Time.deltaTime;
        }

        if (fired)
        {
            if (currentForce + forceDelta <= maxForce)
                currentForce += forceDelta;
            if (currentColliderSize + colliderSizeDelta <= maxColliderSize)
                currentColliderSize += colliderSizeDelta;
            if (currentExplosionSize + explosionSizeDelta <= maxExplosionSize)
                currentExplosionSize += explosionSizeDelta;
            transform.localScale = new Vector3(currentColliderSize, currentColliderSize, 1);
            //GetComponent<CircleCollider2D>().radius = currentSize;
        }
    }

    public void Fire()
    {
        this.gameObject.SetActive(true);
        //direction = dir.normalized;
        //transform.GetComponent<Rigidbody2D>().velocity = speed * direction;
        currentColliderSize = minColliderSize;
        currentExplosionSize = minExplosionSize;
        currentForce = minForce;

        //only does something for server client
        transform.localScale = new Vector3(currentColliderSize, currentColliderSize, 1);
        fired = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isLocalPlayer)
            return;

        if (fired && col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<GunControl>().netId != netIdOfWhoFiredThis)
            {
                Debug.Log("Trying to push " + col.gameObject.name + "netId = " + col.gameObject.GetComponent<GunControl>().netId + "\nwith a rocket force of " + currentForce);
                col.gameObject.transform.GetComponent<GunControl>().RpcApplyRocketForceToSelf(currentForce, (Vector2)(this.transform.position), (Vector2)(this.transform.position - col.gameObject.transform.position));
                CmdSpawnExplosion();
                Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("Ignored Collision between a rocket fired by netId: " + netIdOfWhoFiredThis + " and player netId: " + col.gameObject.transform.GetComponent<GunControl>().netId);
            }
        }

        if (fired && col.gameObject.tag != "Player")
        {

            ContactPoint2D[] contact = new ContactPoint2D[16];// could hold many points of contact so average all to one center point
            Vector2 centerPointOfCollision = new Vector2();
            int i = col.GetContacts(contact);
            int totalNumPointsContacted = i;
            //bad collisions can exist with no points of contact, in this case make rocket's own position be the point of contact
            if (i == 0)
            {
                centerPointOfCollision = (Vector2)transform.position;
            }
            else
            {
                //Debug.Log(i + " point(s) of contact found.");
                for (; i > 0; --i)
                {
                    //Debug.Log("Contact #" + i + " = " + contact[i - 1].point);
                    centerPointOfCollision += contact[i - 1].point;
                }
                centerPointOfCollision /= totalNumPointsContacted;
            }

           // Debug.Log("Average Point of all contacts = " + centerPointOfCollision);
            i = 0;
            Collider2D[] hitColliders = new Collider2D[16];
            Collider2D coll2d;
            ContactFilter2D filter = new ContactFilter2D();//default filter
            Physics2D.OverlapCircle(centerPointOfCollision, currentExplosionSize, filter, hitColliders);
            while (i < hitColliders.Length && hitColliders[i])
            {
                coll2d = hitColliders[i];
                //Debug.Log(hitColliders[i].gameObject.name);
                if (coll2d.tag == "Player")
                {
                    coll2d.gameObject.transform.parent.GetComponent<GunControl>().RpcApplyRocketForceToSelf(currentForce, centerPointOfCollision, centerPointOfCollision - (Vector2)coll2d.gameObject.transform.position);
                }
                else if (coll2d.tag == "PhysObj")
                {
                    coll2d.gameObject.GetComponent<Rigidbody2D>().AddForce(-currentForce * (centerPointOfCollision - (Vector2)coll2d.gameObject.transform.position).normalized + new Vector2(0, 5), ForceMode2D.Impulse);
                }
                ++i;
            }
           // Debug.Log("Num gameObjects within " + currentExplosionSize + " units of centerPoint = " + (i - 1));//i-1 b/c the projectile counts itself

            if (totalNumPointsContacted > 0)
                transform.position = new Vector3(centerPointOfCollision.x, centerPointOfCollision.y, transform.position.z);
            CmdSpawnExplosion();
            fired = false;
            Destroy(this.gameObject);
        }
    }
    [Command]
    void CmdSpawnExplosion()
    {
        //creating an explosion object at the point of collision of the size of the explosion
        GameObject temp = Instantiate(explosion, transform.position, Quaternion.identity);
        temp.GetComponent<explosionController>().scale = currentExplosionSize * 2;
        NetworkServer.Spawn(temp);
    }
}
