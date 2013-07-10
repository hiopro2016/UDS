﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using UDS.Entity;

namespace UDS.SubModule.AFK
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string m = Request.Params["m"];

            switch(m.ToUpper())
            {
                case "AFL":
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                    {
                        try
                        {
                            SqlCommand comm = new SqlCommand();
                            comm.CommandText = "select name, reason from vw_askforleave_list";
                            comm.Connection = conn;

                            List<AFKList> retValues = new List<AFKList>();

                            conn.Open();

                            SqlDataReader reader = comm.ExecuteReader();
                            while (reader.Read())
                            {
                                AFKList r = new AFKList();
                                r.Name = reader.GetString(0);
                                r.Reason = reader.GetString(1);

                                retValues.Add(r);
                            }
                            reader.Close();
                            conn.Close();

                            var jsonSer = new Newtonsoft.Json.JsonSerializer();
                            StringWriter sw = new StringWriter();
                            using (JsonWriter jw = new JsonTextWriter(sw))
                            {
                                jw.DateFormatString = "yyyy年MM月dd日";
                                jw.Formatting = Formatting.Indented;

                                jsonSer.Serialize(jw, retValues);
                            }

                            Response.ContentType = "application/json";

                            Response.Write(sw.ToString());
                            sw.Close();
                        }
                        catch (Exception eX)
                        {
                            Response.StatusCode = 400;
                            Response.Write(eX.Message);
                        }
                        finally
                        { 
                            if(ConnectionState.Open == conn.State)
                                conn.Close();

                            Response.End();
                        }
                    }
                    break;
                case "AFK":
                    break;
            }
        }
    }
}