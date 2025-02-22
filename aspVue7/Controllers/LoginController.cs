using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspVue7.Model;
using System.Data;
using System.Data.SqlClient;//reference from SqlParameter!!!
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Microsoft.AspNetCore.NodeServices;
using System.Security.Cryptography;


namespace aspVue7.Controllers
{
    
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly INodeServices _services;

        public LoginController(INodeServices service)
        {
            _services = service;
        }

        // [HttpPost]
        [HttpPost("[action]")]
        public bool validate([FromBody] usrInfo usrname)
        {
            
            var rng = new Random();
            var model = new BorgWarnerMisSQLContext();
            var usr = usrname.username;
            var testData = model.Database.SqlQuery<usrDBO>($"select Password from dbo.tblWebUser where UsrName='{usr}' ").ToList();
            if(testData.Count==0){
                return false;
            }
             //前端密文解密
            // string oriText = await _services.InvokeAsync<string>("./src/scripts/decrypt",usrname.happyword);
            // string testORI = DecryptByAES(usrname.happyword,"secretkey1234567");
            var validate = PasswordHasher.VerifyHashedPassword(usrname.happyword,testData[0].Password);

            return validate;
        }

        [HttpPost("[action]")]
        public List<regRes> registration([FromBody] regParam info)
        {
            
            var rng = new Random();
            var model = new BorgWarnerMisSQLContext();
            //前端密文解密
            // string oriText = await _services.InvokeAsync<string>("./src/scripts/decrypt",info.happyword);
            //密码加密
            var cipherText = PasswordHasher.HasPassword(info.happyword);
            
            var testData = model.Database.SqlQuery<regRes>($"EXECUTE dbo.QforAccountValidate @name='{info.name}',@username='{info.username}',@pswd='{cipherText}',@token='{info.token}' ").ToList();
            return testData;
        }

        [HttpPost("[action]")]
        public List<mdfyRes> modifyPwd([FromBody] mdfyParam info)
        {
            
            var model = new BorgWarnerMisSQLContext();
            //前端密文解密
            // string oriText = await _services.InvokeAsync<string>("./src/scripts/decrypt",info.newpwd);
            //密码加密
            var cipherText = PasswordHasher.HasPassword(info.newpwd);
            
            var testData = model.Database.SqlQuery<mdfyRes>($"EXECUTE dbo.QforModifyPwd @username='{info.username}',@pswd='{cipherText}',@token='{info.permit}' ").ToList();
            return testData;
        }
        //解密函数
        // public static string DecryptByAES(string input, string key)
        // {
        //     byte[] inputBytes = Convert.FromBase64String(input);
        //     if(key.Length<16)
        //         key = key.PadRight(16, '0');
        //     byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 16));//16
        //     using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        //     {
        //         aesAlg.Key = keyBytes;
        //         aesAlg.IV = keyBytes;
        //         ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        //         using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
        //         {
        //             using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
        //             {
        //                 using (StreamReader srEncrypt = new StreamReader(csEncrypt))
        //                 {
        //                     return srEncrypt.ReadToEnd();
        //                 }
        //             }
        //         }
        //     }
        // }

        [HttpPost("[action]")]
        public List<delRes> deleteUser([FromBody] delParam info)
        {
            
            var model = new BorgWarnerMisSQLContext();
            
            var testData = model.Database.SqlQuery<delRes>($"EXECUTE dbo.QforDeleteUser @username='{info.username}',@token='{info.token}' ").ToList();
            return testData;
        }

        public class usrInfo{
            public string username{get;set;}
            public string happyword{get;set;}
        }

        public class usrDBO{
            public string Password {get;set;}
        }

        //注册参数
        public class regParam{
            public string name{get;set;}
            public string token{get;set;}
            public string username{get;set;}
            public string happyword{get;set;}
        }

        //注册结果
        public class regRes{
            public int 令牌有效性{get;set;}
            public int 令牌使用情况{get;set;}

            public int 用户名使用情况{get;set;}
        }

        //修改密码参数
        public class mdfyParam{
            public string username{get;set;}
            public string permit{get;set;}
            public string newpwd{get;set;}
        }
        //修改密码结果
        public class mdfyRes{
            public int result{get;set;}
        }

        //注销账号参数
        public class delParam{
            public string username{get;set;}
            public string token{get;set;}
        }
        //注销账号结果
        public class delRes{
            public int result{get;set;}
        }
    }
}