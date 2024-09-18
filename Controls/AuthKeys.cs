﻿using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class AuthKeys : Form
    {
        public AuthKeys()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            LoadKeys();

            timer1.Interval = 190;
            timer1.Start();
        }

        private void but_save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void LoadKeys()
        {
            dataGridView1.Rows.Clear();
            foreach (var authKey in MAVAuthKeys.Keys)
            {
                int row = dataGridView1.Rows.Add();
                dataGridView1[FName.Index, row].Value = authKey.Key;
                dataGridView1[Key.Index, row].Value = Convert.ToBase64String(authKey.Value.Key);
            }
        }

        private void Save()
        {
            MAVAuthKeys.Save();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Use.Index)
            {
                MainV2.comPort.setupSigning(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, "",
                    Convert.FromBase64String(dataGridView1[Key.Index, e.RowIndex].Value.ToString()));
            }
        }

        private void but_add_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.Rows.Add();

            string name = "";

            if (InputBox.Show("Name", "Please enter a friendly name", ref name) == DialogResult.OK)
            {
                dataGridView1[FName.Index, row].Value = name;

                string pass = "";

                if (InputBox.Show("Input Seed", "Please enter your pass phrase", ref pass) == DialogResult.OK)
                {
                    var input = InputBox.value;

                    MAVAuthKeys.AddKey(dataGridView1[FName.Index, row].Value.ToString(), input);
                }

                dataGridView1.EndEdit();

                Save();

                LoadKeys();
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            MAVAuthKeys.Keys.Remove(e.Row.Cells[FName.Index].Value.ToString());
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1[Use.Index, e.RowIndex].Value = "Use";
        }

        private void but_disablesigning_Click(object sender, EventArgs e)
        {
            MainV2.comPort.setupSigning(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, "");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var name = "None/Unknown";
            var key = MainV2.comPort.MAV.signingKey;
            if (key != null)
            {
                foreach (var authKey in MAVAuthKeys.Keys)
                {
                    if (authKey.Value.Key.ByteArraysEqual(key))
                    {
                        name = authKey.Key;
                        break;
                    }
                }
            }
            
            lbl_sgnpkts.Text = "Using Key: " + name + ", Signed Packets: " + MainV2.comPort.Mavlink2Signed.ToString();
        }
    }
}
