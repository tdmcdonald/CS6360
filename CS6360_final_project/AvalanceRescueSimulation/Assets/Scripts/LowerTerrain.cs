﻿using UnityEngine;
using System.Collections;
using UnityEngine.XR;
public class LowerTerrain : MonoBehaviour
{
    public bool TestWithMouse = false;
    public Terrain myTerrain;
    public int SmoothArea;
    private int xResolution;
    private int zResolution;
    private float[,] heights;
    private float[,] heightMapBackup;
    public int DeformationTextureNum = 1;
    protected int alphaMapWidth;
    protected int alphaMapHeight;
    protected int numOfAlphaLayers;
    private float[, ,] alphaMapBackup;
    public GameObject shovelObject;
    public Player_script ps;


    void Start()
    {
        xResolution = myTerrain.terrainData.heightmapResolution;
        zResolution = myTerrain.terrainData.heightmapResolution;
        alphaMapWidth = myTerrain.terrainData.alphamapWidth;
        alphaMapHeight = myTerrain.terrainData.alphamapHeight;
        numOfAlphaLayers = myTerrain.terrainData.alphamapLayers;

        GameObject player = GameObject.Find("OVRPlayerController");
        ps = player.GetComponent<Player_script>();
    
        if (Debug.isDebugBuild)
        {
            heights = myTerrain.terrainData.GetHeights (0, 0, xResolution, zResolution);    
            heightMapBackup = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
            alphaMapBackup = myTerrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);   
        }

    }
    void OnApplicationQuit()
    {
        if (Debug.isDebugBuild)
        {
            myTerrain.terrainData.SetHeights(0, 0, heightMapBackup);
            myTerrain.terrainData.SetAlphamaps(0, 0, alphaMapBackup);
        }
    }


    void Update()
    {
        

        //if (ps.CurrentTool == Player_script.SAFTEY_TOOL.SHOVEL)
        if (GameObject.Find("ShovelContainer") != null)
        {

            Debug.Log("EQUIPPED SHOVEL");
            //if (Input.GetJoystickNames().Length < 2)
            //{
            if (!XRDevice.isPresent) {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("clicked");
                        // area middle point x and z, area width, area height, smoothing distance, area height adjust
                        raiselowerTerrainArea(hit.point, 1, 1, SmoothArea, 0.01f);
                        // area middle point x and z, area size, texture ID from terrain textures
                        TextureDeformation(hit.point, 1 * 2f, 0);
                    }
                }
            }
     
            else
            {
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > .8)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Shovel_Controller.shovelPosForRay);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("clicked");
                        // area middle point x and z, area width, area height, smoothing distance, area height adjust
                        raiselowerTerrainArea(Shovel_Controller.shovelPosForRay, 1, 1, SmoothArea, 0.01f);
                        // area middle point x and z, area size, texture ID from terrain textures
                        TextureDeformation(hit.point, 1 * 2f, 0);
                    }
                }

            }
        }
    }


    private void raiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec)
    {
        
        var pointLocalised = myTerrain.transform.InverseTransformPoint(point);

        int areax;
        int areaz;
        smooth += 1;
        float smoothing;
        int terX = (int)((pointLocalised.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((pointLocalised.z / myTerrain.terrainData.size.z) * zResolution);

        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if (terX < 0) terX = 0;
        if (terX > xResolution)    terX = xResolution;
        if (terZ < 0) terZ = 0;
        if (terZ > zResolution)    terZ = zResolution;
        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);
        float y = heights[lenx/2,lenz/2];

        y -= y * incdec;
        for (smoothing=1; smoothing < smooth+1; smoothing++) 
        {
            float multiplier = smoothing / smooth;
            for (areax = (int)(smoothing/2); areax < lenx-(smoothing/2); areax++) 
            {
                for (areaz = (int)(smoothing/2); areaz < lenz-(smoothing/2); areaz++) 
                {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution)) 
                    {
                        heights [areax, areaz] = Mathf.Clamp((float)y*multiplier,0,1);
                    }
                }
            }
        }
        myTerrain.terrainData.SetHeights (terX, terZ, heights);
    }

    protected void TextureDeformation(Vector3 pos, float craterSizeInMeters,int textureIDnum)
    {
        Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, myTerrain, alphaMapWidth, alphaMapHeight);
        int alphaMapCraterWidth = (int)(craterSizeInMeters * (alphaMapWidth / myTerrain.terrainData.size.x));
        int alphaMapCraterLength = (int)(craterSizeInMeters * (alphaMapHeight / myTerrain.terrainData.size.z));
        int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
        int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength/2));
        float[, ,] alphas = myTerrain.terrainData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        for (int i = 0; i < alphaMapCraterLength; i++) //width
        {
            for (int j = 0; j < alphaMapCraterWidth; j++) //height
            {
                circlePosX = (j - (alphaMapCraterWidth / 2)) / (alphaMapWidth / myTerrain.terrainData.size.x);
                circlePosY = (i - (alphaMapCraterLength / 2)) / (alphaMapHeight / myTerrain.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                if (distanceFromCenter < (craterSizeInMeters / 2.0f))
                {
                    for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++)
                    {
                        //could add blending here in the future
                        if (layerCount == textureIDnum)
                        {
                            alphas[i, j, layerCount] = 1;
                        }
                        else
                        {
                            alphas[i, j, layerCount] = 0;
                        }
                    }
                }
            }
        } 
        myTerrain.terrainData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
    }

    protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / myTerrain.terrainData.size.x;
        coord.y = tempCoord.y / myTerrain.terrainData.size.y;
        coord.z = tempCoord.z / myTerrain.terrainData.size.z;
        return coord;
    }

    protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos,Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }     
}