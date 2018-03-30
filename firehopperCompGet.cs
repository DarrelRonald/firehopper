﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace firehopper
{
    public class firehopperCompGet : GH_Component
    {
        public string apiKey;
        public string databaseURL;
        public string databaseNode;
        public bool trigger;

        public string response;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public firehopperCompGet()
          : base("Firehopper Get", "Config",
              "Create header for http request to Google Firebase using Firebase credentials",
              "Firehopper", "Firehopper basic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("apiKey", "K", "apiKey provided by Firebase", GH_ParamAccess.item);
            pManager.AddTextParameter("databaseURL", "U", "databaseURL provided by Firebase", GH_ParamAccess.item);
            pManager.AddTextParameter("databaseNode", "U", "databaseNode in the Firebase", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("trigger", "T", "Trigger the GET request", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("response", "R", "Response received from the Firebase", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override async void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData<string>(0, ref apiKey);
            DA.GetData<string>(1, ref databaseURL);
            DA.GetData<string>(2, ref databaseNode);
            DA.GetData<bool>(3, ref trigger);

            if (trigger == true)
            {
                try
                {
                    response = await getAsync(apiKey, databaseURL, databaseNode);

                    //string nonserializedResponse = await getAsync(apiKey, databaseURL, databaseNode);
                    //JavaScriptSerializer serializer = new JavaScriptSerializer();

                    //serializer.MaxJsonLength = Int32.MaxValue;

                    //if (response != "null")
                    //{
                    //    response = serializer.Deserialize<string>(nonserializedResponse);
                    //}
                    //else
                    //{
                    //    response = "No data stored in the location";
                    //}
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }

            }

            if (trigger == false)
            {
                response = null;
            }

            DA.SetDataList(0, response);
        }

        public static async Task<string> getAsync(string _apiKey, string _databaseURL, string _databaseNode)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_databaseURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "Bearer " + _apiKey);
            HttpResponseMessage res = await httpClient.GetAsync(_databaseURL + _databaseNode + ".json");
            string responseBodyAsText = await res.Content.ReadAsStringAsync();

            return responseBodyAsText;
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e0c92102-0f6f-4434-8de0-f1774eb2d70c"); }
        }
    }
}
