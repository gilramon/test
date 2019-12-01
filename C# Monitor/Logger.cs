﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace SocketServer
{
    class Logger
    {
        class LogClass
        {
            public string msg { get; set; }
            public Color i_Color { get; set; }
            public Color i_TextBackgroundColor { get; set; }
            public DateTime datetimeCreated;
        }

        List<LogClass> LogClassArray = new List<LogClass>();

        string m_LoggerName;
        RichTextBox m_txtBox;
        Button m_ClearTextBoxButton;
        CheckBox m_PauseCheckBox;
        CheckBox m_RecordTofile;
        CheckBox m_StopLogging ; 
        TextBox m_RecognizePatternTextBox;
        TextBox m_RecognizePatternTextBox2;
        TextBox m_RecognizePatternTextBox3;

        string m_log_file_name;

        bool m_PauseText = false;
       // bool m_ExitLog = false;

        public Logger(string i_LoggerName, RichTextBox i_txtBox, Button i_ClearTextBoxButton, CheckBox i_PauseCheckBox, CheckBox i_RecordTofile, TextBox i_RecognizePatternTextBox, TextBox i_RecognizePatternTextBox2, TextBox i_RecognizePatternTextBox3, CheckBox i_StopLogging)
        {
            m_LoggerName = i_LoggerName;
            m_txtBox = i_txtBox;
            m_ClearTextBoxButton = i_ClearTextBoxButton;
            m_PauseCheckBox = i_PauseCheckBox;
            m_RecordTofile = i_RecordTofile;
            //m_RecognizePatternCheckbox = i_RecognizePatternCheckbox;
            m_RecognizePatternTextBox = i_RecognizePatternTextBox;
         //   m_RecognizePatternCheckbox2 = i_RecognizePatternCheckbox2;
            m_RecognizePatternTextBox2 = i_RecognizePatternTextBox2;
            m_RecognizePatternTextBox3 = i_RecognizePatternTextBox3;
            m_StopLogging =i_StopLogging;

            if (m_ClearTextBoxButton != null)
            {
                m_ClearTextBoxButton.Click += new EventHandler(m_ClearTextBoxButton_Click);
            }

            if (m_PauseCheckBox != null)
            {
                m_PauseCheckBox.CheckedChanged += new EventHandler(m_PauseCheckBox_CheckedChanged);
            }

            if (m_RecordTofile != null)
            {
                m_RecordTofile.CheckedChanged += new EventHandler(m_RecordTofile_CheckedChanged);
            }

            //if (m_RecognizePatternCheckbox != null)
            //{
            //    m_RecognizePatternCheckbox.CheckedChanged += new EventHandler(m_RecognizePatternCheckbox_CheckedChanged);
            //}

            //if (m_RecognizePatternCheckbox2 != null)
            //{
            //    m_RecognizePatternCheckbox2.CheckedChanged += new EventHandler(m_RecognizePatternCheckbox2_CheckedChanged);
            //}

            if (m_StopLogging != null)
            {
                m_StopLogging.CheckedChanged += new EventHandler(m_StopLogging_CheckedChanged);
            }

            i_txtBox.MouseUp += richTextBox1_MouseUp;
            i_txtBox.KeyDown += I_txtBox_KeyDown;

        }

        private void I_txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = false;
            }
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {   //click event
                //MessageBox.Show("you got it!");
                ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                MenuItem menuItem = new MenuItem("Cut");
                menuItem.Click += new EventHandler(CutAction);
                contextMenu.MenuItems.Add(menuItem);
                menuItem = new MenuItem("Copy");
                menuItem.Click += new EventHandler(CopyAction);
                contextMenu.MenuItems.Add(menuItem);
                menuItem = new MenuItem("Paste");
                menuItem.Click += new EventHandler(PasteAction);
                contextMenu.MenuItems.Add(menuItem);

                m_txtBox.ContextMenu = contextMenu;
            }
        }
        void CutAction(object sender, EventArgs e)
        {
            m_txtBox.Cut();
        }

        void CopyAction(object sender, EventArgs e)
        {
            Clipboard.SetText(m_txtBox.SelectedText);
        }

        void PasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                m_txtBox.Text += Clipboard.GetText(TextDataFormat.Text).ToString();
            }
        }

        void m_RecognizePatternCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            //if (m_RecognizePatternCheckbox2.Checked)
            //{
            //    m_RecognizePatternCheckbox2.BackColor = Color.Yellow;
            //}
            //else
            //{
            //    m_RecognizePatternCheckbox2.BackColor = default(Color);
            //}
        }

        void m_StopLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (m_StopLogging.Checked)
            {
                m_StopLogging.BackColor = Color.Yellow;
            }
            else
            {
                m_StopLogging.BackColor = default(Color);
            }
        }

        void m_RecognizePatternCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //if (m_RecognizePatternCheckbox.Checked)
            //{
            //    m_RecognizePatternCheckbox.BackColor = Color.Yellow;
            //}
            //else
            //{
            //    m_RecognizePatternCheckbox.BackColor = default(Color);
            //}
        }

        void m_ClearTextBoxButton_Click(object sender, EventArgs e)
        {
            m_txtBox.Invoke(new EventHandler(delegate
            {
                m_txtBox.Text = "";
            }));
        }

        void m_PauseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_PauseCheckBox.Checked)
            {
                m_PauseCheckBox.BackColor = Color.Yellow;

                m_txtBox.BackColor = Color.LightGoldenrodYellow;

                LogMessage(Color.Brown, m_txtBox.BackColor, "Paused !!", true, true);

                m_PauseText = true;
            }
            else
            {
                m_PauseCheckBox.BackColor = default(Color);
                m_txtBox.BackColor = Color.LightGray;
                m_PauseText = false;

                LogMessage(Color.Brown, m_txtBox.BackColor, "Paused Resume", true, true);
            }
        }

        void m_RecordTofile_CheckedChanged(object sender, EventArgs e)
        {
            if (m_RecordTofile.Checked)
            {

                m_RecordTofile.BackColor = Color.Yellow;


                m_log_file_name = "Log_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "_" + m_LoggerName + ".txt";
                try
                {
                    //               Int32 i = 0;
                    while (File.Exists(m_log_file_name))
                    {
                        //                      i++;
                        m_log_file_name = "Log_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "_General_Record" + ".txt";
                    }

                    LogMessage(Color.Brown, m_txtBox.BackColor, "File " + m_log_file_name + " opened in directory \" " +  subPath + "\" \n\n", true, true);
                }
                catch (Exception)
                {
                    LogMessage(Color.Brown, m_txtBox.BackColor, "Can't Open File", true, true);
                }

            }
            else
            {
                m_RecordTofile.BackColor = default(Color);

                LogMessage(Color.Brown, m_txtBox.BackColor, "File " + m_log_file_name + " closed \n\n", true, true);
            }
        }

        bool IsPrinting = false;
        private int NumOfPrints = 0;
        public void LogMessage(Color i_Color, Color i_TextBackgroundColor, string i_msg, bool i_NewLine, bool I_ShowTime)
        {
            try
            {
                if (m_StopLogging != null)
                {
                    if (m_StopLogging.Checked == true)
                    {
                        return;
                    }
                }

                DateTime dt = DateTime.Now;
               



                if (i_NewLine == true)
                {
                    i_msg = i_msg + "\n";
                }


                if (I_ShowTime == true)
                {
                    try
                    {
                        i_msg = "[" + dt.TimeOfDay.ToString().Substring(0, 11) + "] " + i_msg;
                    }
                    catch
                    {
                        i_msg = "[" + dt.TimeOfDay.ToString() + "] " + i_msg;
                    }

                }

                if ( m_RecognizePatternTextBox != null)
                {
                    if (i_msg.Contains(m_RecognizePatternTextBox.Text) && m_RecognizePatternTextBox.Text != string.Empty)
                    {
                        i_TextBackgroundColor = Color.Yellow;
                    }
                }

                if ( m_RecognizePatternTextBox2 != null)
                {
                    if ( i_msg.Contains(m_RecognizePatternTextBox2.Text) && m_RecognizePatternTextBox2.Text != string.Empty)
                    {
                        i_TextBackgroundColor = Color.Cyan;
                    }
                }

                if ( m_RecognizePatternTextBox3 != null)
                {
                    if ( i_msg.Contains(m_RecognizePatternTextBox3.Text) && m_RecognizePatternTextBox3.Text != string.Empty)
                    {
                        i_TextBackgroundColor = Color.Magenta;
                    }
                }

                LogClass logclass = new LogClass();
                logclass.msg = i_msg;
                logclass.i_TextBackgroundColor = i_TextBackgroundColor;
                logclass.i_Color = i_Color;
                logclass.datetimeCreated = DateTime.Now;
                LogClassArray.Add(logclass);

                if (LogClassArray.Count > 1000)
                {
                    //PrintMesseges = false;
                    //LogClassArray.Clear();
                    LogClassArray = LogClassArray.OrderBy(x => x.datetimeCreated).Take(50).ToList();
                }

                if (m_RecordTofile.Checked)
                {
                    // your code goes here

                    bool isExists = System.IO.Directory.Exists(subPath);

                    if (!isExists)
                    {
                        System.IO.Directory.CreateDirectory(subPath);
                    }
                    // This text is always added, making the file longer over time 
                    // if it is not deleted. 
                    using (StreamWriter sw = File.AppendText(subPath + m_log_file_name))
                    {
                        sw.Write(logclass.msg);
                    }
                }


                if (IsPrinting == false && m_PauseText == false && m_txtBox != null)
                {
                    //bool PrintMesseges = true;
                    IsPrinting = true;
                    // Gil: to avoid : Collection was modified; enumeration operation may not execute.

                    m_txtBox.BeginInvoke(new EventHandler(delegate
                    {
                            //LogClass[] LogClassArrayCopy = new LogClass[LogClassArray.Count];
                            List<LogClass> LogClassArrayCopy = new List<LogClass>();
                            for(int i = LogClassArray.Count - 1; i >= 0; i--)  //Gil: Trick to avoid exception list modification
                            {
                                LogClassArrayCopy.Add(LogClassArray[i]);
                            }


                            //foreach (LogClass log in LogClassArray)
                            //{
                            //    LogClassArrayCopy.Add(log);
                            //}
                           // LogClassArray.CopyTo(LogClassArrayCopy);
                            LogClassArray.Clear();
                        // foreach (LogClass log in LogClassArrayCopy)
                        for (int i = LogClassArrayCopy.Count - 1; i >= 0; i--)
                        {
                            LogClass log = LogClassArrayCopy[i];
                                //if (PrintMesseges)
                                {
                            
                                        try
                                        {
                                          //  m_txtBox.SelectionFont = new Font("",10,FontStyle.Regular);
                                            m_txtBox.SelectionColor = log.i_Color;
                                            m_txtBox.SelectionBackColor = log.i_TextBackgroundColor;
                                            m_txtBox.AppendText(log.msg);
                                            m_txtBox.ScrollToCaret();

                                            NumOfPrints++;
                                            if (NumOfPrints > 10000)
                                            {
                                                m_txtBox.Clear();
                                                NumOfPrints = 0;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            m_txtBox.Clear();
                                            m_txtBox.AppendText(ex.ToString());

                                        }
                            
                                }

                            }
                    }));

                    //Lastindex = 0;
                    IsPrinting = false;
                    
                }
            }
            catch(Exception ex)
            {
                try
                {
                    m_txtBox.AppendText(ex.ToString());
                }
                catch
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        string subPath = Directory.GetCurrentDirectory() + "\\Logs\\";
        private void Clear_Log(object sender, EventArgs e)
        {
            m_txtBox.Invoke(new EventHandler(delegate
            {
                m_txtBox.Clear();
            }));
        }

        private void Pause_Log(object sender, EventArgs e)
        {
            LogMessage(Color.Orange, m_txtBox.BackColor, "Paused !!", true, true);
            m_PauseText = true;
        }


        //public void Exit_Log()
        //{
        //    m_ExitLog = false;
        //}


    }
}
