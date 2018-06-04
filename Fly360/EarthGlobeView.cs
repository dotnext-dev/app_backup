using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Urho;
using Urho.Actions;
using Urho.Shapes;
using Xamarin.Forms;

namespace Fly360
{
    public class EarthGlobeView : Urho.Application
    {
        Scene scene;
        Node rootNode;
        Node cameraNode;
        Node selectedNode;
        Camera camera;
        Octree octree;
        DateTime _lastTrackedInput;
        bool _isPaused = false;
        bool _isBusy = false;

        public event EventHandler CitySelected;

        [Preserve]
        public EarthGlobeView(ApplicationOptions options = null) : base(options) { }

        static EarthGlobeView()
        {
            UnhandledException += (s, e) =>
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                
                e.Handled = true;
            };
        }

        protected override void Start()
        {
            base.Start();
            CreateScene();
            SetupViewport();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (_isPaused && (DateTime.Now - _lastTrackedInput) > TimeSpan.FromSeconds(1))
                {
                    AutoRotate();
                }
                return true;
            });
        }

        void CreateScene()
        {
            scene = new Scene();
            octree = scene.CreateComponent<Octree>();

            rootNode = scene.CreateChild(name: "Root");

            var earthNode = rootNode.CreateChild(name: "Earth");
            var earth = earthNode.CreateComponent<Sphere>();
            earth.Color = Urho.Color.Blue;
            earthNode.SetScale(4f);
            earth.SetMaterial(Material.FromImage("earth3.jpg"));

            cameraNode = scene.CreateChild();
            camera = cameraNode.CreateComponent<Camera>();
            cameraNode.Position = new Vector3((float)9.5, (float)9.90, 10) / 1.75f;
            cameraNode.Rotation = new Quaternion(-0.121f, 0.878f, -0.305f, -0.35f);

            Node lightNode = cameraNode.CreateChild();
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Point;
            light.Range = 100;
            light.Brightness = 1.3f;

            var markersNode = rootNode.CreateChild();

            var positions = new Dictionary<string, float[]> {                 { "tokyo", new float[] { 35.683333f, 139.683333f }},//tokyo
                { "amsterdam", new float[] { 52.366667f, 4.900000f } },//amsterdam
                { "delhi", new float[] { 28.538336f, -81.379234f } },//orlando
                { "orlando", new float[] { 28.293155f, 76.919282f } },//delhi             }; 

            foreach (var row in positions.Keys)
                AddMarker(markersNode,
                          lat: positions[row][0],
                          lon: positions[row][1], id: row);

            AutoRotate();
        }

        Node AddMarker(Node parent, float lat, float lon, string id)         {             var markerNode = parent.CreateChild("MarkerRoot_" + id);             var markerHeadNode = markerNode.CreateChild("MarkerHeadModel_" + id);
            var markerTailNode = markerNode.CreateChild("MarkerTailModel_" + id);

            var pinCone = markerTailNode.CreateComponent<Urho.Shapes.Cone>();
            markerTailNode.Scale = new Vector3(0.1f, 0.35f, 0.1f);
            pinCone.Color = Urho.Color.Red;

            var pinHead = markerHeadNode.CreateComponent<Urho.Shapes.Sphere>();
            markerHeadNode.SetScale(0.15f);
            pinHead.SetMaterial(Material.FromImage(id + ".jpg")); 
            GetPositionForHeight(lat, lon, 2.1f, out double x1, out double y1, out double z1);             markerTailNode.Position = new Vector3((float)x1, (float)y1, (float)z1);             markerTailNode.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0),
                   TransformSpace.Parent);
            markerTailNode.Rotate(new Quaternion(90, 0, 0), TransformSpace.Local);

            GetPositionForHeight(lat, lon, 2.2f, out double x2, out double y2, out double z2);
            markerHeadNode.Position = new Vector3((float)x2, (float)y2, (float)z2);
            //markerHeadNode.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0),
            //       TransformSpace.Parent);
            //markerHeadNode.Rotate(new Quaternion(180, 0, 0), TransformSpace.Local);             return markerNode;         }

        private void GetPositionForHeight(float lat, float lon, float height, out double x, out double y, out double z)
        {
            var latRad = lat * (Math.PI / 180);
            var lonRad = -lon * (Math.PI / 180);
            var r = height;

            x = Math.Cos(latRad) * Math.Cos(lonRad) * r;
            y = Math.Sin(latRad) * r;
            z = Math.Cos(latRad) * Math.Sin(lonRad) * r;
        }

        private bool AutoRotate()
        {
            _isPaused = false;
            try
            {
                ResetSelectedNode();
                rootNode.RunActions(new RepeatForever(
                            new RotateBy(
                                duration: 2f,
                                deltaAngleX: 0,
                                deltaAngleY: -15,
                                deltaAngleZ: 0)));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnUpdate(float timeStep)
        {
            if (_isBusy)
                return;
            
            try
            {
                _isBusy = true;
                if (Input.NumTouches >= 1)
                {
                    _lastTrackedInput = DateTime.Now;
                    _isPaused = true;

                    rootNode.RemoveAllActions();
                    var touch = Input.GetTouch(0);


                    var cameraRay = camera.GetScreenRay(
                        (float)touch.Position.X / Graphics.Width,
                        (float)touch.Position.Y / Graphics.Height);

                    var result = octree.RaycastSingle(cameraRay,
                        RayQueryLevel.Triangle, 100, DrawableFlags.Geometry);

                    if (result != null)
                    {
                        var node = result.Value.Node;
                        if (node == selectedNode && selectedNode.Scale.X >= 0.75f)
                        {
                            selectedNode = null;

                            CitySelected?.Invoke(this, EventArgs.Empty);

                            return;
                        }    

                        ResetSelectedNode();
                        if (node.Name.StartsWith("Marker"))
                        {
                            
                            if (node.Parent != null && node.Parent.Name.StartsWith("MarkerRoot"))
                            {
                                node = node.Parent;
                                node = node.Children.First(n => n.Name.StartsWith("MarkerHead"));
                            }
                            if (node.Name.StartsWith("MarkerHead"))
                            {
                                selectedNode = node;
                                node.RunActions(new EaseElasticOut(new ScaleTo(0.7f, 0.75f)));
                            }

                            return;
                        }
                    }

                    //else we move the globe
                    var x = Math.Abs(touch.Delta.X);
                    var y = Math.Abs(touch.Delta.Y);

                    //if (x < y)
                    //    rootNode.Rotate(new Quaternion(0, 0, touch.Delta.Y / 2), TransformSpace.Local);
                    //else
                    rootNode.Rotate(new Quaternion(0, -touch.Delta.X / 3, 0), TransformSpace.Local);

                }
                base.OnUpdate(timeStep);
            }
            catch
            {
                
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void ResetSelectedNode()
        {
            if (selectedNode != null)
            {

                selectedNode.RemoveAllActions();
                selectedNode.RunActions(new EaseElasticIn(new ScaleTo(0.2f, 0.15f)));
                selectedNode = null;
            }
        }

        void SetupViewport()
        {
            var renderer = Renderer;
            var vp = new Viewport(Context, scene, camera, null);
            renderer.SetViewport(0, vp);
        }
    }
}

