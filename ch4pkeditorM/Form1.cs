using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ch4pkeditorM.Data;
using System.Threading;

namespace ch4pkeditorM
{
    public partial class mainForm : Form
    {
        private bool _alive = true;
        private bool _isInForeGround = true;

        public mainForm()
        {
            InitializeComponent();
            initializeEditorFocusMonitor();
            versionLabel.Text = Application.ProductVersion;
        }
        private void initializeEditorFocusMonitor()
        {
            IntPtr handle = Handle;
            Task t = new Task(() =>
            {
                while (_alive)
                {
                    if (_isInForeGround != Core.shared.IsActive(handle))
                    {
                        if (!_isInForeGround && Core.shared.IsInit())
                        {
                            loadMemoryData();
                        }
                        _isInForeGround = !_isInForeGround;
                    }
                    Thread.Sleep(50);
                }
            });
            t.Start();
        }

        private void setControls(bool enable)
        {
            Invoke((MethodInvoker)delegate
            {
                foreach(Control ctrl in Controls)
                {
                    ctrl.Enabled = enable;
                }
            });
        }
        
        private void loadMemoryData()
        {
            ShowMessage("讀取資料中，請稍後。");
            setControls(false);
            Task t = new Task(() =>
            {
                Center.shared.Setup();

                string[] errorMsgs = Core.shared.getErrorMessage();
                if (errorMsgs.Count() > 0)
                {
                    foreach (string msg in errorMsgs)
                    {
                        ShowMessage(msg);
                    }
                }
                else
                {
                    BindListBox(generalCityListBox, Center.shared.CityList, generalCityListBox_SelectedIndexChanged);
                    BindListBox(generalListBox, Center.shared.GeneralList, generalListBox_SelectedIndexChanged);
                    BindListBox(cityListBox, Center.shared.CityList, cityListBox_SelectedIndexChanged);
                    BindListBox(wifeListBox, Center.shared.WifeList, wifeListBox_SelectedIndexChanged);
                    BindComboBox(wifeHusbandNameComboBox, Center.shared.HusbandList, wifeHusbandNameComboBox_SelectedIndexChanged);
                    setControls(true);
                    ShowMessage("讀取完畢。");
                }
            });
            t.Start();
        }

        private void loadGameBtn_Click(object sender, EventArgs e)
        {
            List<Process> process = Core.shared.FindProcess("Ckw95");
            if (process.Count > 0)
            {
                Core.shared.SetProcess(process.ElementAt(0));
                Core.shared.Initialize();
                loadMemoryData();
            }
            else
            {
                ShowMessage("請先開啟遊戲。若已開啟遊戲仍偵測不到，請至論壇回報，並依照網站敘述提供資訊，謝謝。");
            }
        }

        /*
         * Utility
         */
        private void BindListBox(ListBox control, object list, EventHandler evt)
        {
            Invoke((MethodInvoker)delegate
            {
                control.SelectedIndexChanged -= evt;
                control.DataSource = list;
                control.DisplayMember = "Name";
                control.ValueMember = "Order";
                control.SelectedIndex = -1;
                control.SelectedIndexChanged += evt;
            });
        }
        private void BindComboBox(ComboBox control, object list, EventHandler evt)
        {
            Invoke((MethodInvoker)delegate
            {
                control.SelectedIndexChanged -= evt;
                control.DataSource = list;
                control.DisplayMember = "Name";
                control.ValueMember = "Order";
                control.SelectedIndex = -1;
                control.SelectedIndexChanged += evt;
            });
        }
        public void ShowMessage(string text)
        {
            Invoke((MethodInvoker)delegate
            {
                sysInfoTxt.AppendText(text + "\r\n");
            });
        }

        #region General
        private void bindGeneralData(General general)
        {
            Invoke((MethodInvoker)delegate
            {
                generalArcherTxt.Text = ((int)general.Archar).ToString();
                generalAtkTxt.Text = ((int)general.Attack).ToString();
                generalBornTxt.Text = general.GetBirth().ToString();
                generalCavalryTxt.Text = ((int)general.Cavalry).ToString();
                generalExploitTxt.Text = ((int)general.Exploit).ToString();
                generalInfantryTxt.Text = ((int)general.Infantry).ToString();
                generalIntTxt.Text = ((int)general.Intelligence).ToString();
                generalLabel.Text = general.Name;
                generalLoyaltyTxt.Text = ((int)general.Loyalty).ToString();
                generalNavyTxt.Text = ((int)general.Navy).ToString();
                generalPuliticTxt.Text = ((int)general.Pulitic).ToString();
                generalServTxt.Text = general.GetServed().ToString();

                generalAmbushCheckBox.Checked = general.Ambush;
                generalAssaultCheckBox.Checked = general.Assault;
                generalBuildingCheckBox.Checked = general.Building;
                generalBusinessCheckBox.Checked = general.Business;
                generalCultureCheckBox.Checked = general.Culture;
                generalDiplomacyCheckBox.Checked = general.Diplomacy;
                generalFarmCheckBox.Checked = general.Farm;
                generalFireAtkCheckBox.Checked = general.FireAttack;
                generalHireCheckBox.Checked = general.Hire;
                generalMobileCheckBox.Checked = general.Mobile;
                generalRunningFireCheckBox.Checked = general.RunningFire;
                generalSiegeCheckBox.Checked = general.Siege;
            });
        }
        private void saveGeneralData(General general)
        {
            general.Archar = Convert.ToByte(generalArcherTxt.Text, 10);
            general.Attack = Convert.ToByte(generalAtkTxt.Text, 10);
            general.SetBirth(Convert.ToUInt16(generalBornTxt.Text, 10));
            general.Cavalry = Convert.ToByte(generalCavalryTxt.Text, 10);
            general.Exploit = Convert.ToByte(generalExploitTxt.Text, 10);
            general.Infantry = Convert.ToByte(generalInfantryTxt.Text, 10);
            general.Intelligence = Convert.ToByte(generalIntTxt.Text, 10);
            general.Loyalty = Convert.ToByte(generalLoyaltyTxt.Text, 10);
            general.Navy = Convert.ToByte(generalNavyTxt.Text, 10);
            general.Pulitic = Convert.ToByte(generalPuliticTxt.Text, 10);
            general.SetServed(Convert.ToUInt16(generalServTxt.Text, 10));

            general.Ambush = generalAmbushCheckBox.Checked;
            general.Assault = generalAssaultCheckBox.Checked;
            general.Building = generalBuildingCheckBox.Checked;
            general.Business = generalBusinessCheckBox.Checked;
            general.Culture = generalCultureCheckBox.Checked;
            general.Diplomacy = generalDiplomacyCheckBox.Checked;
            general.Farm = generalFarmCheckBox.Checked;
            general.FireAttack = generalFireAtkCheckBox.Checked;
            general.Hire = generalHireCheckBox.Checked;
            general.Mobile = generalMobileCheckBox.Checked;
            general.RunningFire = generalRunningFireCheckBox.Checked;
            general.Siege = generalSiegeCheckBox.Checked;
        }

        private void generalListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int order = (int)generalListBox.SelectedValue;
            bindGeneralData(Center.shared.GeneralList.Find(x => x.Order == order));
        }

        private void generalSaveBtn_Click(object sender, EventArgs e)
        {
            int order = (int)generalListBox.SelectedValue;
            General general = Center.shared.GeneralList.Find(x => x.Order == order);
            saveGeneralData(general);
            byte[] generalData = general.ToByte();
            int offset = order * GeneralMemoryData.BlockDataLength;
            bool result = Core.shared.WriteMemory((IntPtr)(GeneralMemoryData.BlockStart + offset), generalData);
            ShowMessage(result ? "已儲存，請返回遊戲刷新查看。" : "寫入錯誤。");
        }

        private void generalCityListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int order = (int)generalCityListBox.SelectedValue;
            List<General> filteredGeneralList = Center.shared.GeneralList.Where(x => x.StayCity == order).ToList();
            BindListBox(generalListBox, filteredGeneralList, generalListBox_SelectedIndexChanged);
        }
        private void generalNormalFullBtn_Click(object sender, EventArgs e)
        {
            generalPuliticTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalAtkTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalIntTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalLoyaltyTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
        }
        private void generalInternalAffairsFullBtn_Click(object sender, EventArgs e)
        {
            generalFarmCheckBox.Checked = true;
            generalBusinessCheckBox.Checked = true;
            generalBuildingCheckBox.Checked = true;
            generalCultureCheckBox.Checked = true;
            generalDiplomacyCheckBox.Checked = true;
            generalHireCheckBox.Checked = true;
        }

        private void generalBattleFullBtn_Click(object sender, EventArgs e)
        {
            generalMobileCheckBox.Checked = true;
            generalAssaultCheckBox.Checked = true;
            generalRunningFireCheckBox.Checked = true;
            generalFireAtkCheckBox.Checked = true;
            generalSiegeCheckBox.Checked = true;
            generalAmbushCheckBox.Checked = true;
        }

        private void generalArmsFullBtn_Click(object sender, EventArgs e)
        {
            generalInfantryTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalCavalryTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalArcherTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalNavyTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
        }

        private void generalFullBtn_Click(object sender, EventArgs e)
        {
            generalPuliticTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalAtkTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalIntTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalLoyaltyTxt.Text = GeneralDataValidation.Normal.MAX.ToString("d");
            generalInfantryTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalCavalryTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalArcherTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalNavyTxt.Text = GeneralDataValidation.Arms.MAX.ToString("d");
            generalFarmCheckBox.Checked = true;
            generalBusinessCheckBox.Checked = true;
            generalBuildingCheckBox.Checked = true;
            generalCultureCheckBox.Checked = true;
            generalDiplomacyCheckBox.Checked = true;
            generalHireCheckBox.Checked = true;
            generalMobileCheckBox.Checked = true;
            generalAssaultCheckBox.Checked = true;
            generalRunningFireCheckBox.Checked = true;
            generalFireAtkCheckBox.Checked = true;
            generalSiegeCheckBox.Checked = true;
            generalAmbushCheckBox.Checked = true;
        }
        #endregion

        #region City
        private void bindCityData(City city)
        {
            Invoke((MethodInvoker)delegate
            {
                cityAcademicTxt.Text = city.Academic.ToString();
                cityAmberTxt.Text = city.Amber.ToString();
                cityArmyTxt.Text = city.Army.ToString();
                cityArtTxt.Text = city.Art.ToString();
                cityBuildingTxt.Text = city.Building.ToString();
                cityCamelTxt.Text = city.Camel.ToString();
                cityCeladonTxt.Text = city.Celadon.ToString();
                cityChineseMedicineTxt.Text = city.ChineseMedicine.ToString();
                cityCopperTxt.Text = city.Copper.ToString();
                cityCoralTxt.Text = city.Coral.ToString();
                cityCottonFabricTxt.Text = city.CottonFabric.ToString();
                cityCraftTxt.Text = city.Craft.ToString();
                cityDefenceTxt.Text = city.Defence.ToString();
                cityDrawTxt.Text = city.Draw.ToString();
                cityElephantTxt.Text = city.Elephant.ToString();
                cityEmeraldTxt.Text = city.Emerald.ToString();
                cityFarmTxt.Text = city.Farm.ToString();
                cityFoodTxt.Text = city.Food.ToString();
                cityFurTxt.Text = city.Fur.ToString();
                cityGlasswareTxt.Text = city.Glassware.ToString();
                cityGoldTxt.Text = city.Gold.ToString();
                cityHorseTxt.Text = city.Horse.ToString();
                cityHusbandryTxt.Text = city.Husbandry.ToString();
                cityIronTxt.Text = city.Iron.ToString();
                cityIvoryTxt.Text = city.Ivory.ToString();
                cityMedicalTxt.Text = city.Medical.ToString();
                cityMoneyTxt.Text = city.Money.ToString();
                cityNameLabel.Text = city.Name;
                cityOliveOilTxt.Text = city.OliveOil.ToString();
                cityPearlTxt.Text = city.Pearl.ToString();
                cityPotteryTxt.Text = city.Pottery.ToString();
                citySailingTxt.Text = city.Sailing.ToString();
                citySaltTxt.Text = city.Salt.ToString();
                cityScaleTxt.Text = city.Scale.ToString();
                citySeasoningTxt.Text = city.Seasoning.ToString();
                citySilkFabricTxt.Text = city.SilkFabric.ToString();
                citySilverTxt.Text = city.Silver.ToString();
                citySilverwareTxt.Text = city.Silverware.ToString();
                citySpicesTxt.Text = city.Spices.ToString();
                citySugarTxt.Text = city.Sugar.ToString();
                cityTacticsTxt.Text = city.Tactics.ToString();
                cityTeaTxt.Text = city.Tea.ToString();
                cityTurtleShellTxt.Text = city.TurtleShell.ToString();
                cityWeaponTxt.Text = city.Weapon.ToString();
                cityWineTxt.Text = city.Wine.ToString();
                cityWoodTxt.Text = city.Wood.ToString();
                cityWoolTxt.Text = city.Wool.ToString();
            });
        }
        private void saveCityData(City city)
        {
            city.Academic = Convert.ToByte(cityAcademicTxt.Text, 10);
            city.Amber = Convert.ToByte(cityAmberTxt.Text, 10);
            city.Army = Convert.ToUInt16(cityArmyTxt.Text, 10);
            city.Art = Convert.ToByte(cityArtTxt.Text, 10);
            city.Building = Convert.ToByte(cityBuildingTxt.Text, 10);
            city.Camel = Convert.ToByte(cityCamelTxt.Text, 10);
            city.Celadon = Convert.ToByte(cityCeladonTxt.Text, 10);
            city.ChineseMedicine = Convert.ToByte(cityChineseMedicineTxt.Text, 10);
            city.Copper = Convert.ToByte(cityCopperTxt.Text, 10);
            city.Coral = Convert.ToByte(cityCoralTxt.Text, 10);
            city.CottonFabric = Convert.ToByte(cityCottonFabricTxt.Text, 10);
            city.Craft = Convert.ToByte(cityCraftTxt.Text, 10);
            city.Defence = Convert.ToUInt16(cityDefenceTxt.Text, 10);
            city.Draw = Convert.ToByte(cityDrawTxt.Text, 10);
            city.Elephant = Convert.ToByte(cityElephantTxt.Text, 10);
            city.Emerald = Convert.ToByte(cityEmeraldTxt.Text, 10);
            city.Farm = Convert.ToByte(cityFarmTxt.Text, 10);
            city.Food = Convert.ToUInt16(cityFoodTxt.Text, 10);
            city.Fur = Convert.ToByte(cityFurTxt.Text, 10);
            city.Glassware = Convert.ToByte(cityGlasswareTxt.Text, 10);
            city.Gold = Convert.ToByte(cityGoldTxt.Text, 10);
            city.Horse = Convert.ToByte(cityHorseTxt.Text, 10);
            city.Husbandry = Convert.ToByte(cityHusbandryTxt.Text, 10);
            city.Iron = Convert.ToByte(cityIronTxt.Text, 10);
            city.Ivory = Convert.ToByte(cityIvoryTxt.Text, 10);
            city.Medical = Convert.ToByte(cityMedicalTxt.Text, 10);
            city.Money = Convert.ToUInt16(cityMoneyTxt.Text, 10);
            city.OliveOil = Convert.ToByte(cityOliveOilTxt.Text, 10);
            city.Pearl = Convert.ToByte(cityPearlTxt.Text, 10);
            city.Pottery = Convert.ToByte(cityPotteryTxt.Text, 10);
            city.Sailing = Convert.ToByte(citySailingTxt.Text, 10);
            city.Salt = Convert.ToByte(citySaltTxt.Text, 10);
            city.Scale = Convert.ToByte(cityScaleTxt.Text, 10);
            city.Seasoning = Convert.ToByte(citySeasoningTxt.Text, 10);
            city.SilkFabric = Convert.ToByte(citySilkFabricTxt.Text, 10);
            city.Silver = Convert.ToByte(citySilverTxt.Text, 10);
            city.Silverware = Convert.ToByte(citySilverwareTxt.Text, 10);
            city.Spices = Convert.ToByte(citySpicesTxt.Text, 10);
            city.Sugar = Convert.ToByte(citySugarTxt.Text, 10);
            city.Tactics = Convert.ToByte(cityTacticsTxt.Text, 10);
            city.Tea = Convert.ToByte(cityTeaTxt.Text, 10);
            city.TurtleShell = Convert.ToByte(cityTurtleShellTxt.Text, 10);
            city.Weapon = Convert.ToByte(cityWeaponTxt.Text, 10);
            city.Wine = Convert.ToByte(cityWineTxt.Text, 10);
            city.Wood = Convert.ToByte(cityWoodTxt.Text, 10);
            city.Wool = Convert.ToByte(cityWoolTxt.Text, 10);
        }
        private void cityListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int order = (int)cityListBox.SelectedValue;
            bindCityData(Center.shared.CityList.Find(x => x.Order == order));
        }
        private void citySaveBtn_Click(object sender, EventArgs e)
        {
            int order = (int)cityListBox.SelectedValue;
            City city = Center.shared.CityList.Find(x => x.Order == order);
            saveCityData(city);
            byte[] cityData = city.ToByte();
            int offset = order * CityMemoryData.BlockDataLength;
            bool result = Core.shared.WriteMemory((IntPtr)(CityMemoryData.BlockStart + offset), cityData);
            ShowMessage(result ? "已儲存，請返回遊戲刷新查看。" : "寫入錯誤。");
        }
        private void cityNormalFullBtn_Click(object sender, EventArgs e)
        {
            cityMoneyTxt.Text = CityDataValidation.Normal.MAX.ToString("d");
            cityFoodTxt.Text = CityDataValidation.Normal.MAX.ToString("d");
            cityArmyTxt.Text = CityDataValidation.Normal.MAX.ToString("d");
            cityDefenceTxt.Text = CityDataValidation.Defence.MAX.ToString("d");
            cityScaleTxt.Text = CityDataValidation.Scale.MAX.ToString("d");
        }

        private void cityCultureFullBtn_Click(object sender, EventArgs e)
        {
            cityFarmTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityHusbandryTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityWeaponTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityTacticsTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            citySailingTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityBuildingTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityAcademicTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityArtTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityMedicalTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
            cityCraftTxt.Text = CityDataValidation.Culture.MAX.ToString("d");
        }

        private void cityStapleFullBtn_Click(object sender, EventArgs e)
        {
            cityWineTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityOliveOilTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySugarTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityTeaTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySeasoningTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityHorseTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityElephantTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityCamelTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySaltTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityFurTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySilkFabricTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityWoolTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityCottonFabricTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityCeladonTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityPotteryTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySilverwareTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityGlasswareTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityCopperTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySilverTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityGoldTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            citySpicesTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityDrawTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityTurtleShellTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityAmberTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityCoralTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityEmeraldTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityPearlTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityIvoryTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityWoodTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityChineseMedicineTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
            cityIronTxt.Text = CityDataValidation.Specialty.MAX.ToString("d");
        }
        #endregion

        #region Wife
        private void bindWifeData(Wife wife)
        {
            wifeNameLabel.Text = wife.Name;
            wifeBabyBoyRadioBtn.Checked = wife.IsBoyBaby();
            wifeBabyGirlRadioBtn.Checked = wife.IsGirlBaby();
            wifePregnant1stRadioBtn.Checked = wife.IsInPregnentPeriod(0);
            wifePregnant2ndRadioBtn.Checked = wife.IsInPregnentPeriod(1);
            wifePregnant3rdRadioBtn.Checked = wife.IsInPregnentPeriod(2);
            wifePregnant4thRadioBtn.Checked = wife.IsInPregnentPeriod(3);
            wifePregnant5thRadioBtn.Checked = wife.IsInPregnentPeriod(4);
            wifeNotPregnantRadioBtn.Checked = !wife.IsPregnent();
            General husband = Center.shared.GeneralList.Find(x => x.Order == wife.Husband);
            if (husband.IsExist())
            {
                wifeHusbandNameComboBox.SelectedValue = (int)wife.Husband;
            }
        }
        private void saveWifeData(Wife wife)
        {
            int order = (int)wifeHusbandNameComboBox.SelectedValue;
            wife.Husband = Convert.ToUInt16(order);
            int pregnentPeriod = wifePregnant1stRadioBtn.Checked ? 0 : -1;
            pregnentPeriod = wifePregnant2ndRadioBtn.Checked ? 1 : pregnentPeriod;
            pregnentPeriod = wifePregnant3rdRadioBtn.Checked ? 2 : pregnentPeriod;
            pregnentPeriod = wifePregnant4thRadioBtn.Checked ? 3 : pregnentPeriod;
            pregnentPeriod = wifePregnant5thRadioBtn.Checked ? 4 : pregnentPeriod;
            if (pregnentPeriod >= 0)
            {
                wife.Pregnent = WifeMemoryData.GetPregnentByte(wifeBabyGirlRadioBtn.Checked, pregnentPeriod);
            }
            else if(wifeNotPregnantRadioBtn.Checked)
            {
                wife.Pregnent = WifeMemoryData.NoPregnent;
            }
        }
        private void wifeHusbandNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int order = (int)wifeHusbandNameComboBox.SelectedValue;
            int wifeOrder = (int)wifeListBox.SelectedValue;

            Wife wife = Center.shared.WifeList.Find(x => x.Order == wifeOrder);
            wife.Husband = Convert.ToUInt16(order);
        }

        private void wifeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int order = (int)wifeListBox.SelectedValue;
            bindWifeData(Center.shared.WifeList.Find(x => x.Order == order));
        }

        private void wifeSaveBtn_Click(object sender, EventArgs e)
        {
            int order = (int)wifeListBox.SelectedValue;
            Wife wife = Center.shared.WifeList.Find(x => x.Order == order);
            saveWifeData(wife);
            byte[] wifeData = wife.ToByte();
            int offset = order * WifeMemoryData.BlockDataLength;
            bool result = Core.shared.WriteMemory((IntPtr)(WifeMemoryData.BlockStart + offset), wifeData);
            ShowMessage(result ? "已儲存，請返回遊戲刷新查看。" : "寫入錯誤。");
        }


        #endregion
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _alive = false;
        }
    }
}
