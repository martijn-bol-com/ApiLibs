﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibs.General
{
    public abstract class ObjectSearcher
    {
        public Service service;

        public void Search(Service inputService)
        {
            if (service != null)
            {
                return;
            }

            service = inputService;

            foreach (var item in GetType().GetRuntimeProperties())
            {
                if (item.GetValue(this) == null)
                {
                    continue;
                }
                if (item.GetMethod.ReturnType.GetTypeInfo().BaseType == typeof(ObjectSearcher))
                {
                    ObjectSearcher objectSearcher = item.GetValue(this) as ObjectSearcher;
                    objectSearcher.Search(inputService);
                }

                if (item.PropertyType.IsArray)
                {
                    Array a = (Array) item.GetValue(this);
                    foreach (object o in a)
                    {
                        if (o.GetType().GetElementType() == typeof(ObjectSearcher))
                        {
                            (o as ObjectSearcher)?.Search(inputService);
                        }
                    }
                }
            }
        }
    }
}
