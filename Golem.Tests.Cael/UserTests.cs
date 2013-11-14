﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallio.Runtime.Formatting;
using ProtoTest.Golem.Core;
using ProtoTest.Golem.WebDriver;
using Golem.PageObjects.Cael;
using Golem.PageObjects.Mailinator;
using MbUnit.Framework;
using HomePage = Golem.PageObjects.Cael.HomePage;



namespace Golem.Tests.Cael
{
    [TestFixture, DependsOn(typeof(SmokeTests))]
    public class UserTests : WebDriverTestBase
    {
        public static string email1 = Config.GetConfigValue("UserEmail1", "prototestuser1@mailinator.com");
        public static string email2 = Config.GetConfigValue("UserEmail2", "prototestuser2@mailinator.com");
        public static string assessor_email = Config.GetConfigValue("AssessorEmail", "prototestassessor@mailinator.com");
        public static string password = Config.GetConfigValue("Password", "prototest123!!");
        public static string firstName = "ProtoTest";
        public static string lastName = "Tester";
        public static string address1 = "1999 Broadway";
        public static string address2 = "#1410";
        public static string city = "Denver";
        public static string state = "Colorado";
        public static string zip = "80202";
        public static string phone = "3035551234";
        public static string DOB_Month = "January";
        public static string DOB_Day = "1";
        public static string DOB_Year = "1960";

        [Test]
        public void CreateNewUsers()
        {
            // Create 2 students
            CreateStudent("UserEmail1", ref email1);
            CreateStudent("UserEmail2", ref email2);

            // Create assessor
            CreateAssessor("AssessorEmail", ref assessor_email);
        }
        
        [Timeout(0)]
        [Test,DependsOn("CreateNewUsers")]
        public void ActivateUsers()
        {
            // Activate Student and assessor accounts
            ActivateUser(email1);
            ActivateUser(email2);
            ActivateUser(assessor_email, true);
        }

        private void CreateStudent(string configKey, ref string staticRef)
        {
            string email = "prototestuser" + Common.GetRandomString() + "@mailinator.com";

            HomePage.OpenHomePage().
                GoToCreateUserPage().
                CreateUser(email, password, firstName, lastName, address1, address2, city, state, zip, phone, DOB_Month, DOB_Day, DOB_Year);

            // Update the student email
            Common.UpdateConfigFile(configKey, email);
            staticRef = email;
        }

        private void CreateAssessor(string configKey, ref string staticRef)
        {
            string email = "prototestassessor" + Common.GetRandomString() + "@mailinator.com";
            string department = "English";
            string[] subjects = { "Literature (Classics, World, English, etc.)", "Literary Theory" };
            string global_admin = Config.GetConfigValue("GlobalAdmin", "msiwiec@prototest.com");

            Golem.PageObjects.Cael.Kentico.Login(global_admin, password).CreateAssessor(email, password, department, subjects);

            // Update the assessor email
            Common.UpdateConfigFile(configKey, email);
            staticRef = email;
        }

        private void ActivateUser(string user, bool isAssessor=false)
        {
            OpenPage<PageObjects.Mailinator.HomePage>(@"http://mailinator.com/").
               Login(user).
               WaitForEmail("LearningCounts.org", 20).
               OpenEmailWithText("LearningCounts.org").
               ClickTextInBody("sign-in");

            LoginPage loginPage = new LoginPage();
            DashboardPage dp = loginPage.Login(user, password, false, isAssessor);

            if (isAssessor)
            {
                dp.AssessorHeader.SignOut();
            }
            else
            {
                dp.StudentHeader.SignOut();
            }
        }
    }
}
