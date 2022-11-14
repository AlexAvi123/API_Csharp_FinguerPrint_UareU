using System.Data;
using DPUruNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web.Http.Cors;

namespace apifingerControler3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EnrollmentController : ControllerBase
    {
        public static Enrollment1 OnCaptured(string[] captureResult)
        {
            //return("aaa");
            
            try
            {
                // Check capture quality and throw an error if bad.
                //if (!CheckCaptureResult(captureResult)) return;

                // Create bitmap
                /*foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    SendMessage(Action.SendBitmap, CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }*/

                //Enrollment Code:
                //IEnumerable<Fmd> preenrollmentFmds = new List<Fmd>();
                List<Fmd> preenrollmentFmds = new List<Fmd>();
                int count = 0;
                int heightIn = 550;
                //int heightIn = 392;
                //int widthIn = 357;
                int widthIn = 500;
                //int resolutionIn = 500;
                int resolutionIn = 700;
                System.Diagnostics.Debug.WriteLine(captureResult.Length);
                //int resolutionIn = 490000;

                try
                {
                    foreach (string huella in captureResult)
                    {
                        count++;
                        
                        byte[] toIn = Base64UrlTextEncoder.Decode(huella);
                        //byte[] toIn = Convert.FromBase64String(huella);
                        if (captureResult.Length.Equals(0) || widthIn.Equals(0) || heightIn.Equals(0) || resolutionIn.Equals(0))
                        {
                            return new Enrollment1
                            {
                                info ="tu huevada no sirve",
                                xml ="",
                                error =true
                            };
                        }
                        
                        DataResult<Fmd> resultConversion = Importer.ImportFmd(toIn, Constants.Formats.Fmd.DP_PRE_REGISTRATION, Constants.Formats.Fmd.DP_PRE_REGISTRATION);
                        //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromRaw(captureResult, 0, 1, widthIn, heightIn, resolutionIn, Constants.Formats.Fmd.DP_PRE_REGISTRATION);
                        //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromRaw(toIn, 0, 3407615, widthIn, heightIn, resolutionIn, Constants.Formats.Fmd.ANSI);

                        /*System.Diagnostics.Debug.WriteLine("capableeee");
                        System.Diagnostics.Debug.WriteLine(captureResult.Length);
                        System.Diagnostics.Debug.WriteLine(widthIn);
                        System.Diagnostics.Debug.WriteLine(heightIn);
                        System.Diagnostics.Debug.WriteLine(resolutionIn);*/
                        //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);

                        //MessageBox.Show("Huella capturada.  \r\nContador:  " + (count));

                        if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                        {
                            //Reset = true;
                            throw new Exception(resultConversion.ResultCode.ToString() + ", de paso no se transforma bien con resultado ");
                        }
                        System.Diagnostics.Debug.WriteLine(resultConversion.ResultCode);
                        //preenrollmentFmds.Append(resultConversion.Data);
                        preenrollmentFmds.Add(resultConversion.Data);


                    }
                    System.Diagnostics.Debug.WriteLine(count);
                    System.Diagnostics.Debug.WriteLine(preenrollmentFmds);
                    if (count >= 4)
                    {
                        DataResult<Fmd> resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.DP_REGISTRATION, preenrollmentFmds);
                        System.Diagnostics.Debug.WriteLine(resultEnrollment.ResultCode.ToString());
                        if (resultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                        {
                            string ress = Fmd.SerializeXml(resultEnrollment.Data);

                            //preenrollmentFmds.Clear();
                            count = 0;
                            //obj_bal_ForAll.BAL_StoreCustomerFPData("tbl_Finger", txtledgerId.Text, Fmd.SerializeXml(resultEnrollment.Data));
                            //return ("Huella personalizada registrada sastifactoriamente.");
                            
                            
                            return new Enrollment1
                            {
                                info = "Huella personalizada registrada sastifactoriamente",
                                xml = ress,
                                error = false
                            };
                        }
                        else if (resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_INVALID_SET)
                        {
                            //preenrollmentFmds.Clear();
                            count = 0;
                            //return ("El registro de la huella ha fallado.  Vuelva a intentar.");
                            return new Enrollment1
                            {
                                info = "El registro de la huella ha fallado.  Vuelva a intentar.",
                                xml = "",
                                error = true
                            };
                        }else if(resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_NOT_READY)
                        {
                            //preenrollmentFmds.Clear();
                            count = 0;
                            //return ("El registro de la huella ha fallado.  Vuelva a intentar.");
                            return new Enrollment1
                            {
                                info = "Las muestras no fueron lo suficientemente variadas para registrar una huella unica.",
                                xml = "",
                                error = true
                            };
                        }
                    }
                    //return ("Vuelva a colocar su dedo en el lector de huellas. Huellas minimas insuficientes.");
                    return new Enrollment1
                    {
                        info = "Vuelva a colocar su dedo en el lector de huellas. Huellas minimas insuficientes.",
                        xml = "",
                        error = true
                    };
                }
                catch (Exception ex)
                {
                    // Send error message, then close form
                    /*return ("{Error:  " + ex.Message+"}" +
                        "Step: insertar");*/
                    return new Enrollment1
                    {
                        info = "Error durante ingreso de datos. "+ex.Message,
                        xml = "",
                        error = true
                    };
                }
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                /*return ("{Error:  " + ex.Message + "}" +
                    "Step: tansformar");*/
                return new Enrollment1
                {
                    info = "Error durante la preparacion de datos. " + ex.Message,
                    xml = "",
                    error = true
                };
            }

        }

        /*private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/

        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(ILogger<EnrollmentController> logger)
        {
            _logger = logger;
        }

        /*[HttpGet(Name = "GetEnrollment")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/
        [HttpPost]
        public Enrollment1 Post([FromBody] string[] value)
        {
            //string[] resp = value;

            Enrollment1 resp = OnCaptured(value);
            Console.WriteLine(resp);
            return resp;
        }

        /*
        [HttpGet(Name = "GetWeatherForecast2")]
        public IEnumerable<WeatherForecast2> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/

    }

    [ApiController]
    [Route("[controller]")]
    public class VerifyController : ControllerBase
    {
        public static Enrollment1 OnCaptured(string captureResult, FingerDB[] fingerprintDB)
        {
            System.Diagnostics.Debug.WriteLine(captureResult);
            
            
            //System.Diagnostics.Debug.WriteLine(data);
            try
            {
                /*
                // Check capture quality and throw an error if bad.
                if (!CheckCaptureResult(captureResult)) return;

                // Create bitmap
                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    Console.WriteLine(fiv.Height.ToString());
                    Console.WriteLine(fiv.Width.ToString());
                    Console.WriteLine(Encoding.Default.GetString(fiv.RawImage));
                    SendMessage(Action.SendBitmap, CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
                */
                List<Fmd> preenrollmentFmds = new List<Fmd>();
                int count = 0;
                //int heightIn = 392;
                int heightIn = 550;
                int widthIn = 500;
                //int widthIn = 357;
                //int resolutionIn = 500;
                int resolutionIn = 700;
                //int cbeff = 3407615;
                int cbeff = 8;
                System.Diagnostics.Debug.WriteLine(captureResult.Length);
                //Fmd algo = new Fmd()
                //SqlConnection conn = new SqlConnection("data source=AXELHF-PC\\SQLEXPRESS;initial catalog=UsuarioDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;");
                //Verification Code
                try
                {
                    //byte[] bytes = System.Convert.FromBase64String(stringInBase64);
                    // Check capture quality and throw an error if bad.
                    //if (!CheckCaptureResult(captureResult)) return;
                    //SendMessage(Action.SendMessage, "A finger was captured.");
                    //captureResult = Base64Decode(captureResult);
                    //byte[] toIn = Encoding.ASCII.GetBytes(captureResult);
                    
                    //byte[] toIn = Convert.FromBase64String(captureResult);
                    
                    byte[] toIn = Base64UrlTextEncoder.Decode(captureResult);
                    //System.Diagnostics.Debug.WriteLine(ASCIIEncoding.Latin1.GetString(toIn));
                    
                    if (captureResult.Length.Equals(0) || widthIn.Equals(0) || heightIn.Equals(0) || resolutionIn.Equals(0))
                    {
                        return new Enrollment1
                        {
                            info = "No se esta ingresando ninguna huella.",
                            xml = "",
                            error = true
                        };
                    }
                    
                    System.Diagnostics.Debug.WriteLine(Encoding.Default.GetString(toIn));
                    
                    DataResult<Fmd> resultConversion = Importer.ImportFmd(toIn, Constants.Formats.Fmd.DP_VERIFICATION, Constants.Formats.Fmd.DP_VERIFICATION);

                    //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromRaw(toIn, 0, cbeff, widthIn, heightIn, resolutionIn, Constants.Formats.Fmd.DP_VERIFICATION);

                    //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);

                    if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        if (resultConversion.ResultCode != Constants.ResultCode.DP_TOO_SMALL_AREA)
                        {
                            //Reset = true;
                        }
                        throw new Exception(resultConversion.ResultCode.ToString());
                    }
                    System.Diagnostics.Debug.WriteLine(resultConversion.ResultCode.ToString());
                    Fmd firstFinger = resultConversion.Data;
                    //conn.Close();
                    //conn.Open();
                    //SqlDataAdapter cmd = new SqlDataAdapter("Select * from  tblFinger", conn);
                    DataTable dt = new DataTable();
                    dt.Clear();
                    //cmd.Fill(dt);
                    //conn.Close();
                    dt.Columns.Add("LedgerId");
                    dt.Columns.Add("CustomerFinger");
                    List<string> lstledgerIds = new List<string>();
                    count = 0;
                    if (fingerprintDB.Length <= 0)
                    {
                        return new Enrollment1
                        {
                            info = "No existen huellas registradas en el sistema.",
                            xml = "",
                            error = true
                        };
                    }
                    foreach (FingerDB objFinger in fingerprintDB)
                    {
                        DataRow _register = dt.NewRow();
                        if(objFinger.id !=null && objFinger.huella_dactilar != null)
                        {
                            _register["LedgerId"] = objFinger.id;
                            _register["CustomerFinger"] = objFinger.huella_dactilar;
                            dt.Rows.Add(_register);
                        }
                        else
                        {
                            return new Enrollment1
                            {
                                info = "Datos mal ingresado para la lista de registro de huellas. Algun valor esta entrando como nulo.",
                                xml = "",
                                error = true
                            };
                        }
                        
                    }
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            lstledgerIds.Add(dt.Rows[i]["LedgerId"].ToString());
                            //Console.WriteLine(dt.Rows[i]["CustomerFinger"].ToString());
                            Fmd val = Fmd.DeserializeXml(dt.Rows[i]["CustomerFinger"].ToString());
                            System.Diagnostics.Debug.WriteLine(val.Format);

                            CompareResult compare = Comparison.Compare(firstFinger, 0, val, 0);
                            if (compare.ResultCode != Constants.ResultCode.DP_SUCCESS)
                            {
                                //Reset = true;
                                throw new Exception(compare.ResultCode.ToString());
                            }
                            if (Convert.ToDouble(compare.Score.ToString()) == 0)
                            {
                                //MessageBox.Show("Ledger Id is : " + lstledgerIds[i].ToString());
                                count++;
                                return new Enrollment1
                                {
                                    info = "El id de usuario es: " + lstledgerIds[i].ToString(),
                                    xml = ""+ lstledgerIds[i].ToString(),
                                    error = false
                                };
                                //break;
                            }

                        }
                        if (count == 0)
                        {
                            //SendMessage(Action.SendMessage, "Fingerprint not registered.");
                            return new Enrollment1
                            {
                                info = "Huella no registrada.",
                                xml = "",
                                error = false
                            };
                        }

                    }
                    return new Enrollment1
                    {
                        info = "No existen huellas registradas en el sistema.",
                        xml = "",
                        error = true
                    };
                }
                catch (Exception ex)
                {
                    // Send error message, then close form
                    /*return ("{Error:  " + ex.Message+"}" +
                        "Step: insertar");*/
                    return new Enrollment1
                    {
                        info = "Error durante ingreso de datos. " + ex.Message,
                        xml = "",
                        error = true
                    };
                }
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                /*return ("{Error:  " + ex.Message + "}" +
                    "Step: tansformar");*/
                return new Enrollment1
                {
                    info = "Error durante la preparacion de datos. " + ex.Message,
                    xml = "",
                    error = true
                };
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.ASCII.GetString(base64EncodedBytes);
        }

        /*private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };*/

        private readonly ILogger<VerifyController> _logger;

        public VerifyController(ILogger<VerifyController> logger)
        {
            _logger = logger;
        }

        /*[HttpGet(Name = "GetVerify")]
        public string Get()
        {
            return "a";
        }*/
        [HttpPost]
        public Enrollment1 Post([FromBody] Verify entrada)
        {
            Enrollment1 resp = OnCaptured(entrada.HuellaVerify, entrada.HuellaRegistered);
            Console.WriteLine(resp);
            return resp;
        }


    }
}