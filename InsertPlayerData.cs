using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.Samples;

namespace NBCompany.InsertPlayerData
{
    public static class InsertPlayerData
    {
        [FunctionName("InsertPlayerData")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

            dynamic args = context.FunctionArgument;

            var message = $"Hello {context.CallerEntityProfile.Lineage.MasterPlayerAccountId}!";
            log.LogInformation(message);

            dynamic inputValue = null;
            if (args != null && args["selectionData"] != null)
            {
                //Insert Arguemnt into a Dynamic
                inputValue = args["selectionData"];
                log.LogInformation($"{new { input = inputValue} }");

                //Convert Dynamic To String (It's important)
                string JsonString = inputValue.ToString();
                log.LogInformation(JsonString);

                //Convert Json String into PlayerSelectionData class.
                var ConvertedData = JsonConvert.DeserializeObject<PlayerSelectionData>(JsonString)!;
                ConvertedOutput DataSendToClient = new ConvertedOutput();

                //Damage Equation
                Random rd = new Random();
                DataSendToClient.damage = rd.Next(1,100);

                //Insert Data Related
                DataSendToClient.currentMonster = ConvertedData.CurrentMonster;
                DataSendToClient.skillSlotUsed = ConvertedData.SkillSlotUsed;
                DataSendToClient.targetMonsterTeam = ConvertedData.TargetMonsterTeam;
                DataSendToClient.targetMonsterIndexLocation = ConvertedData.TargetMonsterIndexLocation;

                //Assumes this is from Database and Logics
                DataSendToClient.targetMonsterNickname = "Roggo";
                DataSendToClient.skillName = "Slap";
                DataSendToClient.isCriticalHit = false;

                //Return the ConvertedOutput Class, this will give the output in Unity as Json String.
                return new {DataSendToClient};
            }

            return new { messageValue = "Error! No String Value was Found!"};
        }
    }

    public class PlayerSelectionData
    {
        public string CurrentMonster { get; set; }
        public int SkillSlotUsed { get; set; }
        public string TargetMonsterIndexLocation { get; set; }
        public string TargetMonsterTeam { get; set; }
    }

    public class ConvertedOutput
    {
        public int damage { get; set; }
        public string currentMonster { get; set; }
        public int skillSlotUsed { get; set; }
        public string targetMonsterIndexLocation { get; set; }
        public string targetMonsterTeam { get; set; }
        public string skillName { get; set; }
        public string targetMonsterNickname { get; set; }
        public bool isCriticalHit { get; set; }
    }
}
