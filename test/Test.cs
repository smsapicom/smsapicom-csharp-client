﻿using System;
using System.IO;

namespace SMSApi
{
    class Test
    {
        static void Main(string[] args)
        {
            var o = new Test();

//            o.test_sms();
//            o.test_mms();
//            o.test_vms();
//            o.test_hlr();
            o.test_sender();
//            o.test_phonebookgroup();
//            o.test_phonebookcontact();
        }

        public SMSApi.Api.Client client()
        {
            SMSApi.Api.Client client = new SMSApi.Api.Client("test");
            client.SetPasswordRAW("test");

            return client;
        }

        protected SMSApi.Api.SMSFactory SMSFactory;
        protected SMSApi.Api.MMSFactory MMSFactory;
        protected SMSApi.Api.VMSFactory VMSFactory;

        public Test()
        {
            SMSFactory = new SMSApi.Api.SMSFactory(client());
            MMSFactory = new SMSApi.Api.MMSFactory(client());
            VMSFactory = new SMSApi.Api.VMSFactory(client());
        }

        public void test_phonebookgroup()
        {
            var phonebookApi = new SMSApi.Api.PhonebookFactory(client());

            string groupName = "new group123" + DateTime.Now.ToString("his");

            phonebookApi.ActionGroupAdd()
                .SetInfo("ooo info")
                .SetName(groupName)
                .Execute();

            var group = 
                phonebookApi.ActionGroupEdit(groupName)
                    .SetName(groupName + "#edit")
                    .SetInfo("edited info")
                    .Execute();

            group = 
                phonebookApi.ActionGroupGet(group.Name)
                    .Execute();

            var groups = phonebookApi.ActionGroupList().Execute();

            foreach (var g in groups.List)
            {
                System.Console.WriteLine(g.Name + " " + g.NumbersCount + " " + g.Info);
            }

            phonebookApi.ActionGroupDelete(group.Name)
                .Contacts(false)
                .Execute();
        }

        public void test_phonebookcontact() {

            var phonebookApi = new SMSApi.Api.PhonebookFactory(client());

            var number = "694562821";

            var contact = 
                phonebookApi.ActionContactAdd(number)
                    .SetFirstName("Test contact" + DateTime.Now.ToString("his"))
                    .Execute();

            contact =
                phonebookApi.ActionContactEdit(contact.Number)
                    .SetFirstName("Test contact" + DateTime.Now.ToString("his") +"#edited")
                    .SetNumber("694562810")
                    .Execute();

            var contacts = phonebookApi.ActionContactList().Execute();
            foreach (var c in contacts.List)
            {
                System.Console.WriteLine(c.Number + " " + c.FirstName + " " + c.LastName + " " + c.Gender);
            }

            contact = phonebookApi.ActionContactGet(contact.Number).Execute();

            phonebookApi.ActionContactDelete(contact.Number).Execute();
        }

        public void test_sender()
        {
            var senderApi = new SMSApi.Api.SenderFactory(client());

            string name = "testName";

            senderApi.ActionAdd(name).Execute();

//            senderApi.ActionSetDefault("SMSAPI").Execute();

            var senders = senderApi.ActionList().Execute();
            foreach (var sender in senders)
            {
                System.Console.WriteLine("Name: " + sender.Name + " " + sender.Status + " " + sender.Default);
            }

            senderApi.ActionDelete(name).Execute();
        }

        public void test_hlr()
        {
            SMSApi.Api.Action.HLRCheckNumber hlr =
                new SMSApi.Api.Action.HLRCheckNumber();

            var proxy = new SMSApi.Api.ProxyHTTP("http://smsapi.local/api/");

            hlr.Client(client());
            hlr.Proxy(proxy);

            hlr.SetNumber("694562829");
            var r = hlr.Execute();

            foreach (var a in r.List)
            {
                System.Console.WriteLine("ID: " + a.ID + " Number: " + a.Number + " Status: " + a.Status + " " + a.Info);
            }
        }

        public void test_sms()
        {
            var result = 
                SMSFactory.ActionSend()
                    .SetText("test message")
                    .SetTo("694562829")
                    .SetDateSent(DateTime.Now.AddHours(2))
                    .Execute();

            System.Console.WriteLine("Send: " + result.Count);

            string[] ids = new string[result.Count];

            for (int i = 0, l = 0; i < result.List.Count; i++)
            {
                if (!result.List[i].isError() )
                {
                    if (!result.List[i].isFinal())
                    {
                        ids[l] = result.List[i].ID;
                        l++;
                    }
                }
            }

            System.Console.WriteLine("Get:");
            result =
                SMSFactory.ActionGet()
                    .Id(ids)
                    .Execute();

            foreach (var status in result.List)
            {
                System.Console.WriteLine("ID: " + status.ID + " NUmber: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
            }       

            var deleted =
                SMSFactory
                    .ActionDelete()
                        .Id(ids)
                        .Execute();

            System.Console.WriteLine("Deleted: " + deleted.Count);
        }

        public void test_mms()
        {
            var result =
                MMSFactory.ActionSend()
                    .SetSubject("test subject")
                    .SetSmil("<smil><head><layout><root-layout height=\"600\" width=\"425\"/><region id=\"Image\" top=\"0\" left=\"0\" height=\"100%\" width=\"100%\" fit=\"meet\"/></layout></head><body><par dur=\"5000ms\"><img src=\"http://www.smsapi.pl/media/mms.jpg\" region=\"Image\"></img></par></body></smil>")
                    .SetTo("694562829")
                    .SetDateSent(DateTime.Now.AddHours(2))
                    .Execute();

            System.Console.WriteLine("Send: " + result.Count);

            string[] ids = new string[result.Count];

            for (int i = 0, l = 0; i < result.List.Count; i++)
            {
                ids[l] = result.List[i].ID;
                l++;
            }

            System.Console.WriteLine("Get:");
            result =
                MMSFactory.ActionGet()
                    .Id(ids)
                    .Execute();

            foreach (var status in result.List)
            {
                System.Console.WriteLine("ID: " + status.ID + " NUmber: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
            }

            var deleted =
                MMSFactory
                    .ActionDelete()
                        .Id(ids)
                        .Execute();

            System.Console.WriteLine("Deleted: " + deleted.Count);
        }

        public void test_vms()
        {
            Stream file = System.IO.File.OpenRead("tts.wav");

            var result =
                VMSFactory.ActionSend()
                    .SetFile(file)
                    .SetTo("694562829")
                    .SetDateSent(DateTime.Now.AddHours(2))
                    .Execute();

            System.Console.WriteLine("Send: " + result.Count);

            string[] ids = new string[result.Count];

            for (int i = 0, l = 0; i < result.List.Count; i++)
            {
                ids[l] = result.List[i].ID;
                l++;
            }

            System.Console.WriteLine("Get:");
            result =
                VMSFactory.ActionGet()
                    .Id(ids)
                    .Execute();

            foreach (var status in result.List)
            {
                System.Console.WriteLine("ID: " + status.ID + " NUmber: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
            }

            var deleted =
                VMSFactory
                    .ActionDelete()
                        .Id(ids)
                        .Execute();

            System.Console.WriteLine("Deleted: " + deleted.Count);
        }
    }
}