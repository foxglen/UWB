                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using Newtonsoft.Json.Linq;
                                 using UnityEditor;
                                 using UnityEngine;
                                 using UnityEngine.Networking;
                                 using Random = System.Random;
                                 
                                 public class mapGeneration : MonoBehaviour
                                 {
                                     private JObject _jObject;
                                     private Terrain _mapTerrain;
                                     public Road path;
                                     public Road road;
                                     public Road river;
                                     public Road rail;
                                     public float roadHeights = 0.0002f;
                                 
                                     public void Start()
                                     {
                                         _mapTerrain = Terrain.activeTerrain;
                                     }

                                     public void StartGame(String mapInfo)
                                     {
                                         StartCoroutine(GetMapData(mapInfo));
                                     }
                                     private IEnumerator GetMapData(String mapInfo)
                                     {
                                         var unityWebRequest = UnityWebRequest.Get("http://178.128.195.182:1233/mapinfo/"+mapInfo);
                                         yield return unityWebRequest.SendWebRequest();
                                         _jObject = JObject.Parse(unityWebRequest.downloadHandler.text);
                                         UpdateMap();
                                     }
                                 
                                 
                                     private void UpdateMap()
                                     {
                                         var jMap = _jObject["map"]["dimensions"]["map"].First.First;
                                         //resizes the terrain at the start of the update
                                         if (jMap["maxX"] != null && jMap["maxY"] != null)
                                             _mapTerrain.terrainData.size =
                                                 new Vector3(jMap["maxX"].Value<float>() * 2, 2f, jMap["maxY"].Value<float>() * 2);
                                         var jToken = _jObject["map"]["nodes"]["map"].First;
                                         while (jToken.Next != null)
                                         {
                                             //checks for objects with a name such as shops, gates etc.
                                             if (jToken.First["tags"]["name"] != null && jToken.First["unityXValue"] != null &&
                                                 jToken.First["unityYValue"] != null)
                                             {
                                                 var informationSign = (GameObject) Instantiate(Resources.Load("sign_1")); //Defines the gameobject being loaded
                                                 informationSign.AddComponent<ObjectInfo>(); //Adds the object info component
                                                 informationSign.GetComponent<ObjectInfo>().objectInfo =
                                                     jToken.First["tags"].ToString(); //Assigns tags as the object info text
                                                 informationSign.AddComponent<MeshCollider>(); //Adds a mesh colider, used for collision with user viewpoint
                                                 informationSign.AddComponent<Rigidbody>(); //Rigidbody
                                                 informationSign.GetComponent<Rigidbody>().useGravity = true; //Ensures object uses gravity
                                                 informationSign.GetComponent<Rigidbody>().isKinematic = true;
                                                 informationSign.name = jToken.First["tags"]["name"].Value<string>(); //Assigns the name 
                                                 informationSign.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); //Scales the object due to scailing differences
                                                 informationSign.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                     jToken.First["unityYValue"].Value<float>() * 2); //Places the object according to x and y values *2
                                             }
                                             //All other object mapping follows the same form as this ^^^
                                             //checks if we're dealing with a natural object (trees)
                                             else if (jToken.First["tags"]["natural"] != null && jToken.First["unityXValue"] != null &&
                                                      jToken.First["unityYValue"] != null)
                                             {
                                                 var random = new Random().Next(1, 8);  //Random to choose a random tree
                                                 var tree = (GameObject) Instantiate(Resources.Load("tree_"+random));
                                                 tree.AddComponent<ObjectInfo>();
                                                 tree.GetComponent<ObjectInfo>().objectInfo =
                                                     jToken.First["tags"].ToString();
                                                 tree.AddComponent<MeshCollider>();
                                                 tree.AddComponent<Rigidbody>();
                                                 tree.GetComponent<Rigidbody>().useGravity = true;
                                                 tree.GetComponent<Rigidbody>().isKinematic = true;
                                                 tree.name = "Tree";
                                                 tree.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                                 tree.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                     jToken.First["unityYValue"].Value<float>() * 2);
                                                                              }
                                             //checks if we're dealing with a highway node such as streetlights, speedbumps etc
                                             else if (jToken.First["tags"]["highway"] != null && jToken.First["unityXValue"] != null &&
                                                      jToken.First["unityYValue"] != null)
                                             {
                                                 if (jToken.First["tags"]["highway"].Value<string>() == "street_lamp")
                                                 {
                                                     var lamp = (GameObject) Instantiate(Resources.Load("street_lamp_2"));
                                                     lamp.AddComponent<ObjectInfo>();
                                                     lamp.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     lamp.AddComponent<MeshCollider>();
                                                     lamp.AddComponent<Rigidbody>();
                                                     lamp.GetComponent<Rigidbody>().useGravity = true;
                                                     lamp.GetComponent<Rigidbody>().isKinematic = true;
                                                     lamp.name = "Lamp";
                                                     lamp.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                                 else if (jToken.First["tags"]["highway"].Value<string>() == "traffic_signals")
                                                 {
                                                     var trafficLight = (GameObject) Instantiate(Resources.Load("signal_1"));
                                                     trafficLight.AddComponent<ObjectInfo>();
                                                     trafficLight.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     trafficLight.AddComponent<MeshCollider>();
                                                     trafficLight.AddComponent<Rigidbody>();
                                                     trafficLight.GetComponent<Rigidbody>().useGravity = true;
                                                     trafficLight.GetComponent<Rigidbody>().isKinematic = true;
                                                     trafficLight.name = "Traffic Light";
                                                     trafficLight.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                                     trafficLight.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                             }
                                             else if (jToken.First["tags"]["amenity"] != null && jToken.First["unityXValue"] != null &&
                                                      jToken.First["unityYValue"] != null)
                                             {
                                                 if (jToken.First["tags"]["amenity"].Value<string>() == "waste_basket")
                                                 {
                                                     var trashBin = (GameObject) Instantiate(Resources.Load("Trash"));
                                                     trashBin.AddComponent<ObjectInfo>();
                                                     trashBin.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     trashBin.AddComponent<MeshCollider>();
                                                     trashBin.AddComponent<Rigidbody>();
                                                     trashBin.GetComponent<Rigidbody>().useGravity = true;
                                                     trashBin.GetComponent<Rigidbody>().isKinematic = true;
                                                     trashBin.name = "Thrash Can";
                                                     trashBin.transform.localScale = new Vector3(0.4F, 0.4F, 0.4F);
                                                     trashBin.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                                 else if (jToken.First["tags"]["amenity"].Value<string>() == "recycling")
                                                 {
                                                     var recyclingBin = (GameObject) Instantiate(Resources.Load("recycle bin"));
                                                     recyclingBin.AddComponent<ObjectInfo>();
                                                     recyclingBin.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     recyclingBin.AddComponent<MeshCollider>();
                                                     recyclingBin.AddComponent<Rigidbody>();
                                                     recyclingBin.GetComponent<Rigidbody>().useGravity = true;
                                                     recyclingBin.GetComponent<Rigidbody>().isKinematic = true;
                                                     recyclingBin.name = "Recycling Bin";
                                                     recyclingBin.transform.localScale = new Vector3(0.4F, 0.4F, 0.4F);
                                                     recyclingBin.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                                 else if (jToken.First["tags"]["amenity"].Value<string>() == "bench")
                                                 {
                                                     var bench = (GameObject) Instantiate(Resources.Load("bench"));
                                                     bench.AddComponent<ObjectInfo>();
                                                     bench.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     bench.AddComponent<MeshCollider>();
                                                     bench.AddComponent<Rigidbody>();
                                                     bench.GetComponent<Rigidbody>().useGravity = true;
                                                     bench.GetComponent<Rigidbody>().isKinematic = true;
                                                     bench.name = "Bench";
                                                     bench.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
                                                     bench.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                                 else if (jToken.First["tags"]["amenity"].Value<string>() == "toilets")
                                                 {
                                                     var toilet = (GameObject) Instantiate(Resources.Load("sign_1"));
                                                     toilet.AddComponent<ObjectInfo>();
                                                     toilet.GetComponent<ObjectInfo>().objectInfo =
                                                         jToken.First["tags"].ToString();
                                                     toilet.AddComponent<MeshCollider>();
                                                     toilet.AddComponent<Rigidbody>();
                                                     toilet.GetComponent<Rigidbody>().useGravity = true;
                                                     toilet.GetComponent<Rigidbody>().isKinematic = true;
                                                     toilet.name = "Toilets";
                                                     toilet.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
                                                     toilet.transform.position = new Vector3(jToken.First["unityXValue"].Value<float>() * 2, 2,
                                                         jToken.First["unityYValue"].Value<float>() * 2);
                                                 }
                                             }
                                 
                                             jToken = jToken.Next;
                                         }
                                 
                                         UpdateWays(); //Call to update ways to map roads and buildings
                                     }
                                 
                                 
                                     //Updates the ways, including roads, building params, paths.. etc. ---------------------------------------------
                                     private void UpdateWays()
                                     {
                                         //Initialize road objects for mapping
                                         var roadObject = new GameObject();
                                         roadObject.name = "Road";
                                         roadObject.AddComponent<MeshRenderer>();
                                         roadObject.AddComponent<MeshCollider>();
                                         var pathObject = new GameObject();
                                         pathObject.name = "Path";
                                         pathObject.AddComponent<MeshRenderer>();
                                         var riverObject = new GameObject();
                                         riverObject.name = "river";
                                         riverObject.AddComponent<MeshRenderer>();
                                         var railObject = new GameObject();
                                         railObject.name = "railway";
                                         railObject.AddComponent<MeshRenderer>();
                                         var jWays = _jObject["map"]["ways"]["map"];
                                         foreach (var way in jWays)
                                         {
                                             //Updates the buildings -------------------------------
                                             if (way.First["tags"]["building"] != null)
                                             {
                                                 var buildingHeight = 1f; //Defines the default building height
                                                 if (way.First["tags"]["building:levels"] != null)
                                                     buildingHeight *= way.First["tags"]["building:levels"].Value<float>(); //If building has a height associated with it overwrites it
                                                 foreach (var node in way.First["nodes"])
                                                     if (node.Next != null) //Retrieves start and end XY of wall
                                                     {
                                                         var start =
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2);
                                                         var end =
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Next.Value<string>()]["unityXValue"]
                                                                     .Value<float>() * 2, 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Next.Value<string>()]["unityYValue"]
                                                                     .Value<float>() * 2);
                                                         var direction = start.x - end.x > 0 ? start - end : end - start; //Finds what direction wall is oriented
                                                         var angle = Vector3.Angle(direction, transform.forward); 
                                                         var middle = new Vector3((start.x + end.x) / 2, 2 + (buildingHeight / 2), (start.z + end.z) / 2); //Retrieves wall midpoint
                                                         var length = Vector3.Distance(start, end); //Retrieves wall length
                                                         var wall = GameObject.CreatePrimitive(PrimitiveType.Cube); //Creates a primitive cube
                                                         wall.AddComponent<ObjectInfo>(); //Add object info class
                                                         wall.GetComponent<ObjectInfo>().objectInfo =
                                                             way.First["tags"].ToString(); //Sets object info to wall tags
                                                         wall.transform.localScale = new Vector3(0.001f, buildingHeight, length); //Scales the cube to appear as a wall
                                                         wall.transform.position = middle; //Moves it to the walls midpoint
                                                         wall.name = way.First["tags"]["name"] != null ? way.First["tags"]["name"].Value<String>() : "Un-named Building";
                                                         //If the wall is named (In the case of building names) names the wall, otherwise "Un-named Building"
                                                             if(way.First["tags"]["building"].Value<String>() == "retail") {
                                                             wall.GetComponent<Renderer>().material = (Material) AssetDatabase.LoadAssetAtPath(
                                                                 "Assets/Resources/BuildingTextures/Materials/ShopTexture.mat",
                                                                 typeof(Material)); //Assigned it a silver brick texture if it is part of a retail unit
                                                         }
                                                         else
                                                         {
                                                             var random = new Random().Next(6, 14);
                                                             wall.GetComponent<Renderer>().material = (Material) AssetDatabase.LoadAssetAtPath(
                                                                 "Assets/Resources/BuildingTextures/Materials/bt"+random+".mat",
                                                                 typeof(Material));
                                                             //Otherwise assigns random building texture
                                                         }
                                                         wall.GetComponent<Renderer>().material.mainTextureScale = new Vector2(3*length,3*buildingHeight);
                                                         //Scales the texture on to the wall depending on its dimensions
                                                         wall.transform.Rotate(Vector3.up, angle);
                                                         //Wall is rotated to the right position
                                                     }
                                             }
                                             
                                             //Updates the barriers/fences, This has the same methodology as wall/building mapping
                                             else if (way.First["tags"]["barrier"] != null)
                                             {
                                                 var buildingHeight = .3f;
                                                 foreach (var node in way.First["nodes"])
                                                     if (node.Next != null)
                                                     {
                                                         var start =
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2);
                                                         var end =
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Next.Value<string>()]["unityXValue"]
                                                                     .Value<float>() * 2, 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Next.Value<string>()]["unityYValue"]
                                                                     .Value<float>() * 2);
                                 
                                                         var direction = start.x - end.x > 0 ? start - end : end - start;
                                                         var angle = Vector3.Angle(direction, transform.forward);
                                                         var middle = new Vector3((start.x + end.x) / 2, 2 + (buildingHeight / 2), (start.z + end.z) / 2);
                                                         var length = Vector3.Distance(start, end);
                                                         var barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                                         barrier.AddComponent<ObjectInfo>();
                                                         barrier.GetComponent<ObjectInfo>().objectInfo =
                                                             way.First["tags"].ToString();
                                                         barrier.transform.localScale = new Vector3(0.02f, buildingHeight, length);
                                                         barrier.transform.position = middle;
                                                         barrier.name = "barrier";
                                                         barrier.GetComponent<Renderer>().material = (Material) AssetDatabase.LoadAssetAtPath(
                                                             "Assets/Resources/fenceMaterial.mat",
                                                             
                                                             typeof(Material));
                                                         barrier.GetComponent<Renderer>().material.mainTextureScale = new Vector2(3*length,3*buildingHeight);
                                                         barrier.transform.Rotate(Vector3.up, angle);
                                                     }
                                             }
                                             //Checking if we're dealing with a path, road, river or rail all follow the same method
                                             else if (way.First["tags"]["highway"] != null)
                                             {
                                                 //we're dealing with a path
                                                 if (way.First["tags"]["highway"].Value<string>() == "path" ||
                                                     way.First["tags"]["highway"].Value<string>() == "footway" ||
                                                     way.First["tags"]["highway"].Value<string>() == "steps" ||
                                                     way.First["tags"]["highway"].Value<string>() == "pedestrian")
                                                 {
                                                     GameObject pathMapper = Instantiate(roadObject); //Defines pathmapper
                                                     pathMapper.AddComponent<ObjectInfo>(); //Adds object info component
                                                     pathMapper.GetComponent<ObjectInfo>().objectInfo =
                                                         way.First["tags"].ToString(); //Sets object info component to path tag info
                                                     path = pathMapper.AddComponent<Road>();
                                                     path.acceptInput = true; //Sets accept input for use with plugin
                                                     path.mat = (Material) AssetDatabase.LoadAssetAtPath(
                                                         "Assets/Pavement textures pack/pavement 03/pavement pattern 03.mat", typeof(Material)); //Sets path material
                                                     path.roadWidth = 0.1f; //Sets width to be smaller than roads
                                                     path.groundOffset = 0.0001f; //Sets ground offset to ensure path is not under rivers
                                                     var points = new List<Vector3>(); //Creates list of points for mapping
                                                     foreach (var node in way.First["nodes"]) //Cycles through path nodes, adding each to the points list
                                                         points.Add(
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2));
                                 
                                                     if (way.First["tags"]["name"] != null) //If the path is named, gives it a name, otherwise un-named path
                                                     {
                                                         path.name = way.First["tags"]["name"].Value<String>();
                                                     }
                                                     else
                                                     {
                                                         path.name = "Un-named Path";
                                                     }
                                                     path.points = points; //Adds the points list to the path points
                                                     path.Refresh(); //Refreshes the path
                                                     pathMapper.AddComponent<MeshCollider>(); //Adds a mesh-collider so user viewpoint can collide and retrieve information
                                                 }
                                                 //The following methods have the same methodology as pathmapper
                                                 else if (way.First["tags"]["highway"].Value<string>() != "track")
                                                 {
                                                     GameObject roadMapper = Instantiate(roadObject);
                                                     roadMapper.AddComponent<ObjectInfo>();
                                                     roadMapper.GetComponent<ObjectInfo>().objectInfo =
                                                         way.First["tags"].ToString();
                                                     road = roadMapper.AddComponent<Road>();
                                                     road.acceptInput = false;
                                                     if (way.First["tags"]["oneway"] != null)
                                                     {
                                                         road.mat = (Material) AssetDatabase.LoadAssetAtPath("Assets/KajamansRoads/Materials/1RoadMat.mat",
                                                             typeof(Material));
                                                     }
                                                     else
                                                     {
                                                         road.mat = (Material) AssetDatabase.LoadAssetAtPath("Assets/KajamansRoads/Materials/2RoadMat.mat",
                                                             typeof(Material));
                                                     }
                                                     road.roadWidth = 0.3f;
                                                     road.groundOffset = roadHeights;
                                                     var points = new List<Vector3>();
                                                     foreach (var node in way.First["nodes"])
                                                         points.Add(
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2));
                                 
                                                     roadHeights += 0.00001f;
                                                     road.points = points;
                                                     if (way.First["tags"]["name"] != null)
                                                     {
                                                         road.name = way.First["tags"]["name"].Value<String>();
                                                     }
                                                     else
                                                     {
                                                         road.name = "Un-named road";
                                                     }
                                                     road.Refresh();
                                                     roadMapper.AddComponent<MeshCollider>();
                                                 }
                                             }
                                             else if (way.First["tags"]["waterway"] != null)
                                             {
                                                 if (way.First["tags"]["waterway"].Value<string>() == "river" ||
                                                     way.First["tags"]["waterway"].Value<string>() == "canal")
                                                 {
                                                     GameObject riverMapper = Instantiate(riverObject); 
                                                     riverMapper.AddComponent<ObjectInfo>();
                                                     riverMapper.GetComponent<ObjectInfo>().objectInfo =
                                                         way.First["tags"].ToString();
                                                     river = riverMapper.AddComponent<Road>();
                                                     river.acceptInput = false;
                                                     river.mat = (Material) AssetDatabase.LoadAssetAtPath("Assets/Standard Assets/Environment/Water (Basic)/Materials/WaterBasicNighttime.mat",
                                                         typeof(Material));
                                                     river.roadWidth = 0.3f;
                                                     river.groundOffset = 0.001f;
                                                     var points = new List<Vector3>();
                                                     
                                                     foreach (var node in way.First["nodes"])
                                                         points.Add(
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2));
                                                     if (way.First["tags"]["name"] != null)
                                                     {
                                                         river.name = way.First["tags"]["name"].Value<String>();
                                                     }
                                                     else
                                                     {
                                                        river.name = "Un-named River";
                                                     }
                                                     river.points = points;
                                                     river.Refresh();
                                                     riverMapper.AddComponent<MeshCollider>();
                                                 }
                                             }
                                             else if (way.First["tags"]["railway"] != null)
                                             {
                                                 if (way.First["tags"]["railway"].Value<string>() == "rail")
                                                 {
                                                     GameObject railMapper = Instantiate(railObject);
                                                     rail = railMapper.AddComponent<Road>();
                                                     railMapper.AddComponent<ObjectInfo>();
                                                     railMapper.GetComponent<ObjectInfo>().objectInfo =
                                                         way.First["tags"].ToString();
                                                     rail.acceptInput = false;
                                                     rail.mat = (Material) AssetDatabase.LoadAssetAtPath("Assets/Pavement textures pack/pavement 04/pavement pattern 04.mat",
                                                         typeof(Material));
                                                     rail.roadWidth = 0.3f;
                                                     rail.groundOffset = 0.001f;
                                                     var points = new List<Vector3>();
                                                     
                                                     foreach (var node in way.First["nodes"])
                                                         points.Add(
                                                             new Vector3(
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityXValue"].Value<float>() * 2,
                                                                 0,
                                                                 _jObject["map"]["nodes"]["map"][node.Value<string>()]["unityYValue"].Value<float>() *
                                                                 2));
                                                     if (way.First["tags"]["name"] != null)
                                                     {
                                                         rail.name = way.First["tags"]["name"].Value<String>();
                                                     }
                                                     else
                                                     {
                                                         rail.name = "Un-named Rail";
                                                     }
                                                     rail.points = points;
                                                     rail.Refresh();
                                                     railMapper.AddComponent<MeshCollider>();
                                                 }
                                             }
        }
    }
}