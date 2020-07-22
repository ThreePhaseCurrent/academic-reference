using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    class SQLiteViewInTable
    {
        //Вивід в таблицю
        public static void OutInTable(SQLiteCommand command, 
                                      ref SQLiteDataAdapter adapter, ref DataTable table, ref BindingSource bindingSource, 
                                            ref DataGridView dataGridView, ref BindingNavigator bindingNavigator)
        {
            table = new DataTable(); //створення таблиці даних
            bindingSource = new BindingSource(); //створення джерела даних
            adapter = new SQLiteDataAdapter(command); //заповнення адаптеру данними
            adapter.Fill(table); //перенесення данних у таблицю
            bindingSource.DataSource = table; //наповнення джерела данних
            bindingNavigator.BindingSource = bindingSource;  // підв'язка до навігатора
            dataGridView.DataSource = bindingSource;//підв'язка до таблиці
        }
        //Вивід в таблицю з оновленням
        public static void OutInTable(SQLiteCommand command, ref SQLiteDataAdapter adapter, 
                                    ref DataTable table, ref BindingSource bindingSource, ref SQLiteCommandBuilder commandBuilder, 
                                            ref DataGridView dataGridView, ref BindingNavigator bindingNavigator)
        {
            table = new DataTable();//створення таблиці даних
            bindingSource = new BindingSource(); //створення джерела даних
            adapter = new SQLiteDataAdapter(command);//заповнення адаптеру данними
            commandBuilder = new SQLiteCommandBuilder(adapter);
            adapter.Fill(table);//перенесення данних у таблицю
            bindingSource.DataSource = table;//наповнення джерела данних
            bindingNavigator.BindingSource = bindingSource;// підв'язка до навігатора
            dataGridView.DataSource = bindingSource;//підв'язка до таблиці
        }

        public static void SaveDataInBD(SQLiteDataAdapter adapter, DataTable table)
        {
            //оновлення бази даних з таблиці
            try
            {
                adapter.Update(table);
                MessageBox.Show("Изменения в базе данных выполнены!",
                  "Уведомление о результатах", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Изменения в базе данных выполнить не удалось " + ex,
                  "Уведомление о результатах", MessageBoxButtons.OK);
            }
        }

        //додавання данних в базу даних
        public static void AddDataInBD(SQLiteCommand command)
        {
            try
            {
                command.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка додавання даних!");
            }
        }

        //видалення даних з бази даних
        public static void DeleteDataInBD(SQLiteCommand command)
        {
            try
            {
                command.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка видалення даних!" + ex);
            }
        }

        //Редагування даних в базі даних
        public static void UpdateDateInBD(SQLiteCommand command)
        {
            try
            {
                if(command.ExecuteNonQuery() == 0)
                {
                    MessageBox.Show("Такий запис вже зарєєстрований!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Запис з таким кодом вже існує! " + ex);
            }
        }


    }
}
