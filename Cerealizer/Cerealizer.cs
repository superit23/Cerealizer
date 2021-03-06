﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Cerealizer
{
    public interface ICerealizer
    {
        SortedList<string, object> Serial
        { get; set; }

        Type OType
        { get; set; }

        KeyValuePair<string, object> PrimaryKey
        { get; set; }

        object this[string s]
        { get; set; }

        object this[int i]
        { get; set; }

        SortedList<string, object> Serialize();

        bool IsPropertyExcluded(PropertyInfo pinfo);

        object Deserialize();

        object DefaultProperties();
    }

    public interface ICerealizer<T> : ICerealizer
    {
        void Deserialize(out T obj);
    }

    public class Exclude : Attribute
    {
        public Exclude()
        {

        }
    }

    /// <summary>
    /// Serializes and deserializes an object into/from its properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cerealizer<T> : ICerealizer<T> where T : new()
    {
        private T tObj = default(T);

        public SortedList<string, object> Serial
        { get; set; }

        public Type OType
        { get; set; }

        public KeyValuePair<string, object> PrimaryKey
        { get; set; }


        public Cerealizer(T obj)
        {
            tObj = obj;
            Serialize();
        }

        public Cerealizer()
        {
            tObj = new T();
            DefaultProperties();
        }


        public SortedList<string, object> Serialize()
        {
            SortedList<string, object> toRet = new SortedList<string, object>();

            foreach (PropertyInfo pinfo in typeof(T).GetProperties())
            {
                if (IsPropertyPrimaryKey(pinfo))
                {
                    PrimaryKey = new KeyValuePair<string, object>(pinfo.Name, pinfo.GetValue(tObj));
                }

                if (!IsPropertyExcluded(pinfo))
                {
                    toRet.Add(pinfo.Name, pinfo.GetValue(tObj));
                }

            }

            OType = tObj.GetType();
            Serial = toRet;

            return toRet;
        }

        public object Deserialize()
        {
            T toRet = new T();
            foreach (PropertyInfo pinfo in typeof(T).GetProperties())
            {
                if (!IsPropertyExcluded(pinfo))
                {
                    pinfo.SetValue(toRet, Serial[pinfo.Name]);
                }

            }

            return (object)toRet;
        }

        public void Deserialize(out T obj)
        {
            obj = (T)Deserialize();
        }

        public bool IsPropertyExcluded(PropertyInfo pinfo)
        {
            foreach (object attribute in pinfo.GetCustomAttributes(true))
            {
                if (attribute is Exclude)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPropertyPrimaryKey(PropertyInfo pinfo)
        {
            foreach (object attribute in pinfo.GetCustomAttributes(true))
            {
                if (attribute is PrimaryKey)
                {
                    return true;
                }
            }
            return false;
        }

        public object DefaultProperties()
        {
            foreach (PropertyInfo pinfo in typeof(T).GetProperties())
            {
                if (!IsPropertyExcluded(pinfo))
                {
                    if (pinfo.PropertyType != typeof(string))
                        pinfo.SetValue(tObj, Activator.CreateInstance(pinfo.PropertyType));
                    else
                        pinfo.SetValue(tObj, "");
                }

            }
            Serialize();

            return (object)tObj;

        }

        public object this[int i]
        {
            get
            {
                return Serial[Serial.Keys.ToList()[i]];
            }

            set
            {
                Serial[Serial.Keys.ToList()[i]] = value;
            }
        }

        public object this[string s]
        {
            get
            {
                return Serial[s];
            }

            set
            {
                Serial[s] = value;
            }
        }

    }

    public class PrimaryKey : Attribute
    {
        public PrimaryKey()
        {

        }
    }


}

