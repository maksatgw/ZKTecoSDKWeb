using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using zkemkeeper;
using ZKWEBAPI.Models;

namespace ZKWEBAPI.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            CZKEMClass zk = new CZKEMClass();
            string ipAddress = "172.20.117.15"; //local ip
            int port = 4370;

            try
            {
                if (zk.Connect_Net(ipAddress, port))
                {
                    //Check linked docs for manual.
                    //https://yemenpolice-ac.gov.ye/SD/Library/5/51asdfbnm.pdf

                    zk.RegEvent(1, 65535);

                    //Created a list with AttendanceLog type for returning data.
                    List<AttendanceLog> attendanceLogs = new List<AttendanceLog>();

                    //Check docs for params. 
                    string enrollNumber = "";
                    int verifyMode = 0, inOutMode = 0;
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;


                    if (zk.ReadGeneralLogData(1))
                    {
                        //While reading data it will create objects and add them to the list
                        while (zk.SSR_GetGeneralLogData(1, out enrollNumber, out verifyMode, out inOutMode, out year, out month, out day, out hour, out minute, out second, 1))
                        {
                            attendanceLogs.Add(new AttendanceLog
                            {
                                User = enrollNumber,
                                Date = new DateTime(year, month, day, hour, minute, second),
                            });
                        }
                    }

                    //Here we return the data
                    return Ok(attendanceLogs);
                }
                else
                {
                    return BadRequest("Connection failed.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                zk.Disconnect();
            }
        }
    }
}
