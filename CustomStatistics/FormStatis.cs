using CustomStatistics.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CustomStatistics
{
    public partial class FormStatis : Form
    {

        public string OraDbConn = ConfigurationManager.AppSettings["OraDBcons"]; //引用System.Configuration.dll
        public string XianYu= ConfigurationManager.AppSettings["XianYu"];

        //初始化xmldoc对象
        XmlDocument xmldoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();

        //2021-01-13 14:58:27 
        //实例化存储统计信息的对象
        CustomKing customKing = new CustomKing();
        //实例化存储所有数据的集合
        List<CustomXmlEntity> customXmlEntities = new List<CustomXmlEntity>();

        public FormStatis()
        {
            InitializeComponent();
            //优化展示样式
            this.dataGridViewMaster(dataGridView1);

            //一上来查询条件窗体是折叠的
            //splitContainer2.Panel1Collapsed = true;

            //忽略XML文档里面的注释
            settings.IgnoreComments = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //最大化窗体
            this.WindowState = FormWindowState.Maximized;

            //将XMl中的配置内容渲染到实体类中
            getAllConfig();

            //清空一上来的选中行
            this.dataGridView1.ClearSelection();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 获取所有的XML配置信息
        /// </summary>
        private void getAllConfig() {
            //读取总配置XML文件
            XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + @"CustomXML\Statis.xml", settings);
            xmldoc.Load(reader);
            //得到根节点thesqlcustom
            XmlNode xmlnode = xmldoc.SelectSingleNode("thesqlcustom");
            //得到根节点的所有子节点
            XmlNodeList xmlnodelist = xmlnode.ChildNodes;
            //创建Datatable对象用于赋值图表
            DataTable dataTableStatis = new DataTable();
            dataTableStatis.Columns.Add("selectName");
            dataTableStatis.Columns.Add("sql_statement");
            dataTableStatis.Columns.Add("selectid");
            foreach (XmlNode thesqlcustomNode in xmlnodelist)
            {
                //定义存储配置数据的对象
                CustomXmlEntity customXmlEntity = new CustomXmlEntity();
                customXmlEntity.CustomId = thesqlcustomNode.Attributes["id"].Value;
                customXmlEntity.CustomSql_statement = thesqlcustomNode.SelectSingleNode("sql_statement").InnerText;
                customXmlEntity.CustomTitle = thesqlcustomNode.SelectSingleNode("sql_statement").InnerText;
                //默认都不需要查询条件
                customXmlEntity.IsNeedConditionKey = "0";

                //创建行并添加进datatable中
                DataRow dataRow = dataTableStatis.NewRow();
                dataRow["selectid"] = thesqlcustomNode.Attributes["id"].Value;
                dataRow["selectName"] = thesqlcustomNode.SelectSingleNode("title").InnerText;
                dataRow["sql_statement"] = thesqlcustomNode.SelectSingleNode("sql_statement").InnerText;
                dataTableStatis.Rows.Add(dataRow);

                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xmlele = (XmlElement)thesqlcustomNode;
                //获取节点下的参数---即获取select节点
                XmlNodeList getSelectNode = xmlele.GetElementsByTagName("isNeedCondition");
                if (getSelectNode.Count != 0)
                {
                    if (getSelectNode.Item(0).Attributes["key"].Value.Equals("1"))
                    {
                        customXmlEntity.IsNeedConditionKey = "1";
                        //定义存储查询条件配置的集合
                        List<SelectCustomNode> selectCustomNodes = new List<SelectCustomNode>();
                        //获取查询条件项
                        foreach (XmlNode conditionXmlNode in thesqlcustomNode.SelectSingleNode("isNeedCondition").ChildNodes)
                        {
                            SelectCustomNode selectCustomNode = new SelectCustomNode();
                            selectCustomNode.ConditionId = conditionXmlNode.Attributes["cid"].Value;
                            selectCustomNode.ConditionType = conditionXmlNode.SelectSingleNode("ConditionType").InnerText;
                            selectCustomNode.ConditionName = conditionXmlNode.SelectSingleNode("ConditionName").InnerText;
                            selectCustomNode.ConditionDBName = conditionXmlNode.SelectSingleNode("ConditionDBName").InnerText;

                            //表值代称
                            selectCustomNode.ConditionDBPronoun = conditionXmlNode.SelectSingleNode("ConditionDBPronoun") == null ? "" : conditionXmlNode.SelectSingleNode("ConditionDBPronoun").InnerText + ".";

                            selectCustomNode.IsLike = conditionXmlNode.SelectSingleNode("isLike") == null ? "0" : conditionXmlNode.SelectSingleNode("isLike").InnerText;

                            selectCustomNode.ConditionListText = conditionXmlNode.SelectSingleNode("ConditionListText") == null ? "" : conditionXmlNode.SelectSingleNode("ConditionListText").InnerText;


                            selectCustomNodes.Add(selectCustomNode);
                        }
                        customXmlEntity.SelectConditions = selectCustomNodes;
                    }
                }
                customXmlEntities.Add(customXmlEntity);//获取完查询基础数据项目后
            }
            //将所有数据加载进总变量中
            customKing.CustomXmls = customXmlEntities;
            //在dataview中展示所有查询
            this.dataGridView1.DataSource = dataTableStatis;

        }

        /// <summary>
        /// 2020-12-11 10:28:53 yuxz
        /// 初始化所进行的操作
        /// 将全部xml信息读取进缓存中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX2_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 行被点击选中事件
        /// 2021-07-23 17:20:11 yuxz
        /// 这个必须要优化,暂时没有优化,需要将取值相关的代码进行整合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            //选择的时候先清空生成的所有控件
            this.flowLayoutPanel1.Controls.Clear();

            //获取ID
            String getSelectid = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();

            //读取路径下xml文件
            XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + @"CustomXML\Statis.xml", settings);
            xmldoc.Load(reader);
            //得到根节点thesqlcustom
            XmlNode xmlnode = xmldoc.SelectSingleNode("thesqlcustom");
            //得到根节点的所有子节点
            XmlNodeList xmlnodelist = xmlnode.ChildNodes;

            foreach (XmlNode thesqlcustomNode in xmlnodelist)
            {

                //根据选中行的id获取我们要获取的查询条件节点数据
                if (thesqlcustomNode.Attributes["id"].Value.Equals(getSelectid))
                {
                    // 将节点转换为元素，便于得到节点的属性值
                    XmlElement xmlele = (XmlElement)thesqlcustomNode;
                    //获取节点下的参数---即获取select节点
                    XmlNodeList getSelectNode = xmlele.GetElementsByTagName("isNeedCondition");
                    
                    if (getSelectNode.Count == 0)
                    {
                        //splitContainer2.Panel1Collapsed = true;
                    }
                    else
                    {
                        //获取isNeedCondition节点
                        if (getSelectNode.Item(0).Attributes["key"].Value.Equals("1"))
                        {
                            //splitContainer2.Panel1Collapsed = false;

                            //获取xml文件中配置的查询条件
                            //根据配置的条件生成相应的窗体
                            foreach (XmlNode childNode in thesqlcustomNode.SelectSingleNode("isNeedCondition").ChildNodes)
                            {
                               
                                //获取这个查询条件的类型
                                String  getConditionType= childNode.SelectSingleNode(("ConditionType")).InnerText;

                                //text为文本框
                                if(getConditionType.Equals("text")){

                                    Label label = new Label();
                                    label.Text = childNode.SelectSingleNode("ConditionName").InnerText;

                                    TextBox textBox = new TextBox();
                                    textBox.Name = childNode.SelectSingleNode("ConditionDBName").InnerText;

                                    this.flowLayoutPanel1.Controls.Add(label);
                                    this.flowLayoutPanel1.Controls.Add(textBox);

                                }

                                //tests为下拉框
                                if (getConditionType.Equals("texts"))
                                {
                                    Label label = new Label();
                                    label.Text = childNode.SelectSingleNode("ConditionName").InnerText;

                                    ComboBox comboBox = new ComboBox();
                                    comboBox.Name= childNode.SelectSingleNode("ConditionDBName").InnerText;
                                    //默认选空
                                    comboBox.Items.Add("");

                                    //根据逗号分隔
                                    string[] strArr = childNode.SelectSingleNode("ConditionListText").InnerText.Split(',');
                                    foreach (String str in strArr)
                                    {
                                        comboBox.Items.Add(str);
                                    }

                                    //默认选中第一个
                                    comboBox.SelectedIndex =0;

                                    this.flowLayoutPanel1.Controls.Add(label);
                                    this.flowLayoutPanel1.Controls.Add(comboBox);
                                }

                                //date是时间框
                                //当是日期类型时,要生成两个窗体,作为查询的时间区间
                                if (getConditionType.Equals("date")) {

                                    Label label = new Label();
                                    label.Text = childNode.SelectSingleNode("ConditionName").InnerText;
                                    //开始时间
                                    DateTimePicker beginDateTimePicker = new DateTimePicker();
                                    beginDateTimePicker.Tag = childNode.SelectSingleNode("ConditionDBName").InnerText;
                                    beginDateTimePicker.Name="begin"+ childNode.SelectSingleNode("ConditionDBName").InnerText;
                                    //结束时间
                                    DateTimePicker endDateTimePicker = new DateTimePicker();
                                    endDateTimePicker.Tag = childNode.SelectSingleNode("ConditionDBName").InnerText;
                                    endDateTimePicker.Name="end" + childNode.SelectSingleNode("ConditionDBName").InnerText;
                                    //设置一个不会溢出的宽高
                                    beginDateTimePicker.Width = 135;
                                    endDateTimePicker.Width = 135;

                                    this.flowLayoutPanel1.Controls.Add(label);

                                    this.flowLayoutPanel1.Controls.Add(beginDateTimePicker);
                                    this.flowLayoutPanel1.Controls.Add(endDateTimePicker);

                                }

                                //checktext是多选框
                                if (getConditionType.Equals("checktext")) {
                                    Label label = new Label();
                                    label.Text = childNode.SelectSingleNode("ConditionName").InnerText;
                                    this.flowLayoutPanel1.Controls.Add(label);

                                    //根据逗号分隔
                                    string[] strArr = childNode.SelectSingleNode("ConditionListText").InnerText.Split(',');
                                    foreach (String str in strArr)
                                    {

                                        CheckBox checkBox = new CheckBox();
                                        checkBox.Name = str;
                                        checkBox.Text = str;
                                        checkBox.Width = 70;
                                        this.flowLayoutPanel1.Controls.Add(checkBox);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //鼠标右击事件
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }


        /// <summary>
        /// 生成按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_MouseClick(object sender, MouseEventArgs e)
        {
            //遍历控件下的所有控件
            foreach (Control control in this.flowLayoutPanel1.Controls)
            {
                if (control is TextBox)
                {
                    //2020-12-25 10:48:30 
                    //这里要循环判断节点---根据节点的字段命名来拼接sql串


                    String getSelectid = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();


                    if (((TextBox)control).Name.Equals("RIS_NO"))
                    {

                        //你要取得的值
                        string s = ((TextBox)control).Text;
                        MessageBox.Show(s);
                    }


                }
            }
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonQueryPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (superGridControl1.PrimaryGrid.Rows.Count > 0)
                {
                    //string localFilePath, fileNameExt, newFileName, FilePath; 
                    SaveFileDialog sfd = new SaveFileDialog();
                    //设置文件类型 
                    sfd.Filter = "Excel文件（*.xls）|*.xls";
                    //设置默认文件类型显示顺序 
                    sfd.FilterIndex = 1;

                    //保存对话框是否记忆上次打开的目录 
                    sfd.RestoreDirectory = true;

                    //点了保存按钮进入 
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                        //存在先删除
                        if (File.Exists(localFilePath) == true)
                        {
                            File.Delete(localFilePath);
                        }
                        //创建空的
                        FileInfo fi = new FileInfo(localFilePath);
                        FileStream fs = fi.Create();
                        fs.Close();
                        fs.Dispose();
                        Aspose.Cells.Workbook wk = new Aspose.Cells.Workbook(localFilePath);
                        //string sj = string.Format("{0}到{1}", dateTimeInput1.Value.ToString("yyyy-MM-dd"), dateTimeInput2.Value.ToString("yyyy-MM-dd"));
                        wk.Worksheets[0].Name ="导出列表";
                        DataTable dt = superGridControl1.PrimaryGrid.DataSource as DataTable;
                        AddExcelSHeet(wk, dt);
                        wk.Save(localFilePath);
                        MessageBox.Show("导出成功：" + localFilePath, "报告列表数据导出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出错误：" + ex.ToString());
            }

        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
             
            //快速获取某个属性的集合
            //var studentCodeList = StudentList.Select(t => t.StudentCode).ToList();


        }

        private void buttonX4_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel1Collapsed = false;
        }

        /// <summary>
        /// 查询按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX5_Click(object sender, EventArgs e)
        {
            String selectSql = String.Empty;

            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请确保选中了一个查询项目！");
            }
            else {

                //获取这个查询的id
                String getSelectid = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                
                foreach (CustomXmlEntity customXmlEntity in customXmlEntities)
                {
                    //确定到查询信息实体类
                    if (customXmlEntity.CustomId.Equals(getSelectid))
                    {
                        //获取配置的sql语句
                        selectSql = customXmlEntity.CustomSql_statement;

                        if (customXmlEntity.IsNeedConditionKey.Equals("1")) {

                            //在where 1=1之后插入值
                            //或
                            int whereFuckSmall = selectSql.IndexOf("where 1=1");

                            int whereFuckBig = selectSql.IndexOf("WHERE 1=1");

                            String andSql = getControlAllConditionText(customXmlEntity.SelectConditions);

                            if (whereFuckSmall > 0)
                            {

                                selectSql = selectSql.Insert(whereFuckSmall + 9, andSql);

                            } else if (whereFuckBig>0) {

                                selectSql = selectSql.Insert(whereFuckBig + 9, andSql);
                            
                            }
                        }
                    }
                }

                //MessageBox.Show(selectSql);
                // MessageBox.Show(selectSql);
                //查询时清空表列
                this.superGridControl1.PrimaryGrid.Columns.Clear();
                this.superGridControl1.PrimaryGrid.DataSource = OraDbCon.GetDataTable(selectSql, OraDbConn);

               
            }
        }


        /// <summary>
        /// 根据查询条件拼接and语句
        /// </summary>
        /// <param name="selectConditions"></param>
        /// <returns></returns>
        private String getControlAllConditionText(List<SelectCustomNode> selectConditions) {

            String selectConditionAnd = ""; 

            foreach (SelectCustomNode nodeItem in selectConditions)
            {
                //如果是文本框类型
                if (nodeItem.ConditionType.Equals("text"))
                {
                    if (!getControlTextByControlName(nodeItem.ConditionDBName).Equals(""))
                    {
                        //使用获取where 1=1位置拼接的方式
                        if (nodeItem.IsLike.Equals("1"))
                        {

                            selectConditionAnd = selectConditionAnd+ " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " like " + "'%" + getControlTextByControlName(nodeItem.ConditionDBName) + "%'";

                        }
                        else
                        {

                            selectConditionAnd = selectConditionAnd + " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " = " + "'" + getControlTextByControlName(nodeItem.ConditionDBName) + "'";

                        }

                    }
                }

                //如果是日期类型
                if (nodeItem.ConditionType.Equals("date"))
                {
                    String getBeginTime = getControlTextByControlName("begin" + nodeItem.ConditionDBName);
                    String getEndTime = getControlTextByControlName("end" + nodeItem.ConditionDBName);

                    selectConditionAnd = selectConditionAnd + " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " >= to_date('" + getBeginTime + " 00:00:00 " + "', 'yyyy-MM-dd HH24:MI:SS')";
                    selectConditionAnd = selectConditionAnd + " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " <= to_date('" + getEndTime + " 23:59:59 " + "', 'yyyy-MM-dd HH24:MI:SS')";

                }

                //如果是下拉框类型
                if (nodeItem.ConditionType.Equals("texts"))
                {

                    if (!getControlTextByControlName(nodeItem.ConditionDBName).Equals(""))
                    {

                        //使用获取where 1=1位置拼接的方式
                        if (nodeItem.IsLike.Equals("1"))
                        {

                            selectConditionAnd = selectConditionAnd + " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " like " + "'%" + getControlTextByControlName(nodeItem.ConditionDBName) + "%'";

                        }
                        else
                        {

                            selectConditionAnd = selectConditionAnd + " and " + nodeItem.ConditionDBPronoun + nodeItem.ConditionDBName + " = " + "'" + getControlTextByControlName(nodeItem.ConditionDBName) + "'";

                        }
                    }
                }
            }
            return selectConditionAnd;
        }

        /// <summary>
        /// 根据控件名称获取控件的值
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        private String getControlTextByControlName(String controlName) {

            String s=String.Empty;
            //遍历控件下的所有控件
            foreach (Control control in this.flowLayoutPanel1.Controls)
            {
                if (control is TextBox)
                {
                    //2020-12-25 10:48:30 
                    //这里要循环判断节点---根据节点的字段命名来拼接sql串
                    if (((TextBox)control).Name.Equals(controlName))
                    {
                        //需要取得的值
                         s = ((TextBox)control).Text;
                    }
                }

                if (control is DateTimePicker) {

                    if (((DateTimePicker)control).Name.Equals(controlName)) {
                        s = ((DateTimePicker)control).Value.ToString("yyyy-MM-dd");
                    }
                }

                if(control is ComboBox){
                    if (((ComboBox)control).Name.Equals(controlName))
                    {
                        //需要取得的值
                        if (((ComboBox)control).SelectedItem != null) {
                            s = ((ComboBox)control).SelectedItem.ToString();
                        }
                        
                    }
                }

            }
            return s;
        }

        /// <summary>
        /// 优化dataview数据展示样式
        /// </summary>
        /// <param name="dataGridView"></param>
        private void dataGridViewMaster(DataGridView dataGridView)
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridView.AllowUserToAddRows = false;//默认不可增加
            dataGridView.AllowUserToResizeRows = false; //禁止改变行高
            dataGridView.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridView.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            //自动列宽一定要关闭！不然容不下大量列
            //dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //背景色为白色
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            //dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            //dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(230)))), ((int)(((byte)(180)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = true;
            //设置行高
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //默认选中整行
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void AddExcelSHeet(Aspose.Cells.Workbook wk, DataTable dt)
        {
            DataRow[] werow = dt.Select();
            if (werow.Length > 0)
            {
                int i = 0;
                for (int j = 0; j < superGridControl1.PrimaryGrid.Columns.Count; j++)
                {
                    wk.Worksheets[0].Cells[i, j].PutValue(superGridControl1.PrimaryGrid.Columns[j].HeaderText);
                }
                foreach (DataRow rows in werow)
                {
                    i = i + 1;
                    for (int j = 0; j < rows.ItemArray.Length; j++)
                    {
                        wk.Worksheets[0].Cells[i, j].PutValue(rows[j].ToString());
                    }
                }
            }
        }

        # region 基于windows的excel的报表导出，暂时弃用
        /// <summary>
        /// 基于windows的excel的报表导出，暂时弃用
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="myDGV">控件DataGridView</param>
        private void ExportExcels(string fileName, DataGridView myDGV)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return; //被点了取消
            }
            saveFileName = saveDialog.FileName;

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
                                                                                                                                  //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁
            MessageBox.Show("文件： " + fileName + ".xls 保存成功", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        /// <summary>
        /// 导航栏关于按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            if (XianYu.Equals("1")) {

                AboutBoxYu aboutBoxYu = new AboutBoxYu();

                aboutBoxYu.ShowDialog();
            }

        }


        /// <summary>
        /// 给dataview报表加载左侧号码行数---性能存在一些影响---需优化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{
            //    dataGridView1.Rows[i].HeaderCell.Value = (i+1).ToString();
            //}
        }

    }
}
