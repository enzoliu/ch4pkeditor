using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch4pkeditorM.Data
{
    public class Center
    {
        /*
         * Singleton instance
         */
        public static Center shared = new Center();

        /*
         * Variables
         */
        public List<Country> CountryList = new List<Country>();
        public List<City> CityList = new List<City>();
        public List<General> GeneralList = new List<General>();
        public List<General> HusbandList = new List<General>();
        public List<Wife> WifeList = new List<Wife>();

        public void Setup()
        {
            shared.SetupGeneralList();
            shared.SetupCountryList();
            shared.SetupCityList();
            shared.SetupWifeList();
        }

        public void SetupGeneralList()
        {
            setup(
                GeneralList,
                GeneralMemoryData.BlockStart,
                GeneralMemoryData.BlockDataLength,
                GeneralMemoryData.MaxNumber
            );
            setup(
                HusbandList,
                GeneralMemoryData.BlockStart,
                GeneralMemoryData.BlockDataLength,
                GeneralMemoryData.MaxNumber
            );
        }

        public void SetupCityList()
        {
            setup(
                CityList,
                CityMemoryData.BlockStart,
                CityMemoryData.BlockDataLength,
                CityMemoryData.MaxNumber
            );
        }

        public void SetupCountryList()
        {
            setup(
                CountryList,
                CountryMemoryData.BlockStart,
                CountryMemoryData.BlockDataLength,
                CountryMemoryData.MaxNumber
            );
        }

        public void SetupWifeList()
        {
            setup(
                WifeList,
                WifeMemoryData.BlockStart,
                WifeMemoryData.BlockDataLength,
                WifeMemoryData.MaxNumber
            );
        }

        private void setup<T>(List<T> list, int position, int length, int max)
        {
            list.Clear();
            byte[] bs = Core.shared.readMemory((IntPtr)(position), length * max);
            for (int i = 0; i < max; i++)
            {
                int offset = i * length;
                
                if (bs.Count() == 0)
                {
                    continue;
                }

                byte[] temp = new byte[length];
                Buffer.BlockCopy(bs, offset, temp, 0, length);
                T obj = (T)Activator.CreateInstance(typeof(T), new object[] { temp, i });
                if (obj is ICH4PKObject)
                {
                    if (!((ICH4PKObject)obj).IsExist())
                    {
                        continue;
                    }
                    list.Add(obj);
                }
            }
        }
    }
}
