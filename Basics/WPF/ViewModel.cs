using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CardLifeAltLaunch.Properties;

namespace CardLifeAltLaunch
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void onPropertyChanged(string aPropName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(aPropName));
            }
        }

        protected bool SetProperty<T>(ref T aRefVal, T aVal, params string[] aPropNameArray)
        {
            if (!aRefVal.Equals(aVal))
            {
                aRefVal = aVal;

                if (aPropNameArray != null)
                {
                    foreach (var aPropName in aPropNameArray)
                    {
                        onPropertyChanged(aPropName);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        public object this[string aKey]

        {
            get
            {
                try {
                    return Resources.ResourceManager.GetObject(aKey);
                } catch(Exception)
                {
                    return aKey;
                }
            }

        }

    }
}

