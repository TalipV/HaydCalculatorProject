using HaydCalculator.Entities;
using HaydCalculator.Enums;
using System;

namespace HaydCalculator
{
    public partial class MainForm : Form
    {
        private HaydCalculatorFactory haydCalculatorFactory = new HaydCalculatorFactory();

        public MainForm()
        {
            InitializeComponent();

            this.fromDateDateTimePicker.Value = DateTime.Now.Date;
            this.toDateDateTimePicker.Value = DateTime.Now.Date.AddDays(3);

            this.haydTypeComboBox.DataSource = Enum.GetValues(typeof(EFlowAppearanceColor));
            this.haydTypeComboBox.SelectedItem = EFlowAppearanceColor.Red;
        }

        private void clearDataButton_Click(object sender, EventArgs e)
        {
            this.data.Clear();
            this.menstruationListBox.DataSource = null;
            this.istihadaListBox.DataSource = null;
            this.inputListBox.DataSource = null;

            this.fromDateDateTimePicker.Enabled = true;
        }

        List<FlowDataEntity> data = new List<FlowDataEntity>();

        private FlowDataEntity getHaydTimeDataFromInput()
        {
            DateTime fromDate = this.fromDateDateTimePicker.Value;
            DateTime toDate = this.toDateDateTimePicker.Value;
            EFlowAppearanceColor haydDataType = (EFlowAppearanceColor)this.haydTypeComboBox.SelectedValue;

            if (toDate <= fromDate)
            {
                throw new Exception("Ungültige Eingabe!");
            }

            return new FlowDataEntity()
            {
                FromDateTime = fromDate,
                ToDateTime = toDate,
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = haydDataType }
            };
        }


        private void acceptInputButton_Click(object sender, EventArgs e)
        {
            FlowDataEntity newHaydTimeData = null;

            try
            {
                newHaydTimeData = getHaydTimeDataFromInput();
                data.Add(newHaydTimeData);

                RecalculateData(data);

                this.fromDateDateTimePicker.Value = newHaydTimeData.ToDateTime;
                this.toDateDateTimePicker.Value = newHaydTimeData.ToDateTime.AddDays(3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (newHaydTimeData != null)
                {
                    data.Remove(newHaydTimeData);
                }
            }
            finally
            {
                this.fromDateDateTimePicker.Enabled = data.None();
            }
            
            this.menstruationListBox.DataSource = null;
            this.menstruationListBox.DataSource = haydCalculatorFactory.MenstruationOLD;
            this.istihadaListBox.DataSource = null;
            this.istihadaListBox.DataSource = haydCalculatorFactory.IstihadaLst;
            this.inputListBox.DataSource = null;
            this.inputListBox.DataSource = haydCalculatorFactory.DataList;
        }

        private void RecalculateData(List<FlowDataEntity> data)
        {
            throw new NotImplementedException();
        }
    }
}