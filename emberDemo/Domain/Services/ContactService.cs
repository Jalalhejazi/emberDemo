﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Models;
using System.IO;
using System.Web.Script.Serialization;

namespace Domain.Services
{
    public class ContactService : IContactService
    {
        const string path = "c:\\temp\\contacts.json";
        static object o = new object();

        #region IContactService Members

        public List<Contact> All
        {
            get
            {
                if (_all == null)
                {
                    _all = GetAll();
                }
                return _all;
            }
        }
        public List<Contact> _all;

        public List<Models.Contact> GetAll()
        {
            lock (o)
            {
                using (var reader = File.OpenText(path))
                {
                    var data = reader.ReadToEnd();

                    if (!String.IsNullOrEmpty(data))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var result = jss.Deserialize<List<Contact>>(data);
                        return result;
                    }
                }
            }
            return new List<Contact>();
        }

        public int Add(Models.Contact contact)
        {
            var maxId = 0;

            if (All.Count > 0)
            {
                maxId = All.Max(c => c.id);
            }

            contact.id = maxId + 1;
            All.Add(contact);

            Save();

            return contact.id;
        }

        public void Update(Models.Contact contact)
        {
            All.RemoveAll(c => c.id == contact.id);
            All.Add(contact);
            Save();
        }

        public void Delete(Models.Contact contact)
        {
            All.RemoveAll(c => c.id == contact.id);

            Save();
        }

        public void Clear()
        {
            All.Clear();
            Save();
        }

        private void Save()
        {
            var jss = new JavaScriptSerializer();
            var data = jss.Serialize(All);

            lock (o)
            {
                using (var writer = new StreamWriter(File.Open(path, FileMode.Create)))
                {
                    writer.Write(data);
                }
            }
        }


        #endregion

    }
}
