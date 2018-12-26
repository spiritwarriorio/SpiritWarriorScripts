using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool SHOW_COLLIDER = true; //$$

    public static LevelManager Instance { set; get; }

    public GameObject entrada, salida;
    private const float DISTANCE_BEFORE_SPAWN = 150.0f;
    private const int INITIAL_SEGMENTS = 5;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 9;
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int y1, y2, y3;
    public bool Iniciado = false;
    private int NumeroDeSpawns = 5;
    private int Contador = 0;
    private int zona;

    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocksS = new List<Piece>();
    public List<Piece> jumpsB = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> longblocks2 = new List<Piece>();
    public List<Piece> longblockscabeza = new List<Piece>();
    public List<Piece> jumpsPA = new List<Piece>();
    public List<Piece> jumpsPALiquido = new List<Piece>();
    public List<Piece> jumpsCC = new List<Piece>();
    public List<Piece> jumpsCCLiquido = new List<Piece>();
    public List<Piece> jumpsT = new List<Piece>();
    public List<Piece> jumpsA = new List<Piece>();
    public List<Piece> jumpsALiquido = new List<Piece>();
    public List<Piece> jumpsABolos = new List<Piece>();
    public List<Piece> jumpsMRC = new List<Piece>();
    public List<Piece> jumpsMRD = new List<Piece>();
    public List<Piece> jumpsMR = new List<Piece>();
    public List<Piece> slidesB = new List<Piece>();
    public List<Piece> slidesA = new List<Piece>();
    public List<Piece> slidesPA = new List<Piece>();
    public List<Piece> slidesCC = new List<Piece>();
    public List<Piece> slidesT = new List<Piece>();
    public List<Piece> slidesTBlocker = new List<Piece>();
    public List<Piece> slidesMR = new List<Piece>();
    public List<Piece> blocksA = new List<Piece>();
    public List<Piece> blocksPA = new List<Piece>();
    public List<Piece> blocksCC = new List<Piece>();
    public List<Piece> blocksCCcarros = new List<Piece>();
    public List<Piece> blocksT = new List<Piece>();
    public List<Piece> blocksPABig = new List<Piece>();
    public List<Piece> blocksPALittle = new List<Piece>();
    public List<Piece> FloorZones1 = new List<Piece>();

    public List<Piece> ArcadeSound = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); //All the pieces in the pool

    //List of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        zona = 0;
    }
    private void Start()
    {
        Contador = 0;
        if(GameManager.Once == false)
        {
            for (int i = 0; i < INITIAL_SEGMENTS; i++)
            {
                if (i < INITIAL_TRANSITION_SEGMENTS)
                {
                    SpawnTransition();
                }
                else
                {
                    Iniciado = true;
                    SpawnSegment();
                }
            }
        }
    }

    private void Update()
    {
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
        {
            Contador++;
            SpawnSegment();
            if (Contador >= NumeroDeSpawns)
            {
                zona = 0;
                Contador = 0;
            }           
        }

        if(amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments[amountOfActiveSegments - 1].DeSpawn();
            amountOfActiveSegments--;
        }
        if (amountOfActiveSegments2 >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments2[amountOfActiveSegments2 - 1].DeSpawn();
            amountOfActiveSegments2--;
        }
        if (amountOfActiveSegments3 >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments3[amountOfActiveSegments3 - 1].DeSpawn();
            amountOfActiveSegments3--;
        }
    }

    private void SpawnSegment()
    {
        switch (zona)
        {
            case 0:
                List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
                int id = Random.Range(0, possibleSeg.Count);

                Segment s = GetSegment(id, false);

                y1 = s.endY1;
                y2 = s.endY2;
                y3 = s.endY3;

                s.transform.SetParent(transform);
                s.transform.localPosition = Vector3.forward * currentSpawnZ;

                currentSpawnZ += s.lenght;
                amountOfActiveSegments++;
                s.Spawn();
                break;
        }
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.lenght;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

        if(s == null)
        {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();

            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s);
        }
        else
        {
            segments.Remove(s);
            segments.Insert(0, s);
        }

        return s;
    }

    public Piece GetPiece(PieceType pt, int visualIndex)
    {
        Piece p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject go = null;
            if (pt == PieceType.ramp)
            {
                go = ramps[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblock)
            {
                go = longblocks[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblock2)
            {
                go = longblocks2[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpPA)
            {
                go = jumpsPA[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpPALiquido)
            {
                go = jumpsPALiquido[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpCC)
            {
                go = jumpsCC[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpCCLiquido)
            {
                go = jumpsCCLiquido[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpALiquido)
            {
                go = jumpsALiquido[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpABolos)
            {
                go = jumpsABolos[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpA)
            {
                go = jumpsA[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpT)
            {
                go = jumpsT[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpMR)
            {
                go = jumpsMR[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpMRD)
            {
                go = jumpsMRD[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpMRC)
            {
                go = jumpsMRC[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideMR)
            {
                go = slidesMR[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideA)
            {
                go = slidesA[visualIndex].gameObject;
            }
            else if (pt == PieceType.slidePA)
            {
                go = slidesPA[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideCC)
            {
                go = slidesCC[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideT)
            {
                go = slidesT[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideTBlocker)
            {
                go = slidesTBlocker[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockPA)
            {
                go = blocksPA[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockCC)
            {
                go = blocksCC[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockA)
            {
                go = blocksA[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockCCcarros)
            {
                go = blocksCCcarros[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockT)
            {
                go = blocksT[visualIndex].gameObject;
            }
            else if (pt == PieceType.jumpB)
            {
                go = jumpsB[visualIndex].gameObject;
            }
            else if (pt == PieceType.slideB)
            {
                go = slidesB[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblockS)
            {
                go = longblocksS[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblockCabeza)
            {
                go = longblockscabeza[visualIndex].gameObject;
            }
            else if (pt == PieceType.floorZone1)
            {
                go = FloorZones1[visualIndex].gameObject;
            }
            else if (pt == PieceType.floorZone2)
            {
                go = FloorZones2[visualIndex].gameObject;
            }
            else if (pt == PieceType.floorZone3)
            {
                go = FloorZones3[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockPABig)
            {
                go = blocksPABig[visualIndex].gameObject;
            }
            else if (pt == PieceType.blockPALittle)
            {
                go = blocksPALittle[visualIndex].gameObject;
            }
            else if (pt == PieceType.ArcadeSound)
            {
                go = ArcadeSound[visualIndex].gameObject;
            }

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
        }

        return p;
    }

}
