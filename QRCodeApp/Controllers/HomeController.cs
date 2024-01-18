using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using QRCodeApp.Models;
using QRCodeApp.Services;

namespace QRCodeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Login request)
        {
            var user = await _userService.QueryUserByLogin(request);
            if (user != null)
            {
                if (request.Username == user.Username && request.Password == user.Password)
                {
                    return RedirectToAction("QRView", "Home", new { username = user.Username, userId = user.UserId, firstname = user.FirstName });
                }
            }
            
            return View();
        }


        public async Task<IActionResult> QRView(string username, string userId, string firstname)
        {

            string qrText = await GenerateId(username, userId);
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap Bitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = Bitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            ViewBag.QrCodeUri = QrUri;
            ViewBag.FirstName = firstname;
            return View();
        }

        public async Task<IActionResult> QRCodeVerify(string v)
        {
            var valueBytes = System.Convert.FromBase64String(v);
            string decodedString = Encoding.UTF8.GetString(valueBytes);
            int actionIdResponse = await _userService.UpdateUserAction(decodedString);
            return View();
        }


        public async Task<string> GenerateId(string username, string userId)
        {
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("yyyyMMddHHmm");
            Random rnd = new Random();
            long lastSix = rnd.Next(1, 9999);
            string referenceId = userId + " " + formattedDate + lastSix;

            int actionIdResponse = await _userService.CreateUserAction(username, referenceId);

            var refParamByte = Encoding.UTF8.GetBytes(referenceId);
            var refParamLink = Convert.ToBase64String(refParamByte);
            string url = "http://10.28.128.201:5125/Home/QRCodeVerify?v=" + refParamLink;
            return url;
        }
    }

    public static class BitmapToArray
    {
        public static Byte[] BitmapToByteArray(this Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}