﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace ApiLibs.Todoist
{
    public class Note
    {
        public int is_deleted { get; set; }
        public int is_archived { get; set; }
        public string content { get; set; }
        public int posted_uid { get; set; }
        public object uids_to_notify { get; set; }
        public int item_id { get; set; }
        public int project_id { get; set; }
        public int id { get; set; }
        public string posted { get; set; }
    }

    public class Label
    {
        public int item_order { get; set; }
        public int is_deleted { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public int id { get; set; }
        public int uid { get; set; }
    }

    public class LiveNotification
    {
        public int completed_last_month { get; set; }
        public string promo_img { get; set; }
        public int created { get; set; }
        public object seq_no { get; set; }
        public string notification_key { get; set; }
        public string notification_type { get; set; }
        public int is_deleted { get; set; }
        public int karma_level { get; set; }
        public int? completed_in_days { get; set; }
        public int? completed_tasks { get; set; }
    }

    public class User
    {
        public string start_page { get; set; }
        public bool is_premium { get; set; }
        public int sort_order { get; set; }
        public string full_name { get; set; }
        public int auto_reminder { get; set; }
        public bool has_push_reminders { get; set; }
        public int id { get; set; }
        public int next_week { get; set; }
        public int completed_count { get; set; }
        public List<object> tz_offset { get; set; }
        public string timezone { get; set; }
        public string email { get; set; }
        public double karma { get; set; }
        public int start_day { get; set; }
        public int date_format { get; set; }
        public int inbox_project { get; set; }
        public int time_format { get; set; }
        public object image_id { get; set; }
        public int beta { get; set; }
        public string karma_trend { get; set; }
        public object business_account_id { get; set; }
        public object mobile_number { get; set; }
        public object mobile_host { get; set; }
        public int is_dummy { get; set; }
        public string premium_until { get; set; }
        public bool guide_mode { get; set; }
        public string join_date { get; set; }
        public string token { get; set; }
        public bool is_biz_admin { get; set; }
        public string default_reminder { get; set; }
    }

    public class Filter
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public int item_order { get; set; }
        public string query { get; set; }
        public int is_deleted { get; set; }
        public int id { get; set; }
    }

    public class Item
    {
        public string due_date { get; set; }
        public int day_order { get; set; }
        public int? assigned_by_uid { get; set; }
        public string due_date_utc { get; set; }
        public int is_archived { get; set; }
        public List<int> labels { get; set; }
        public int? sync_id { get; set; }
        public int in_history { get; set; }
        public string date_added { get; set; }
        public int @checked { get; set; }
        public string date_lang { get; set; }
        [JsonProperty]
        public int id { get; set; }
        public string content { get; set; }
        public int indent { get; set; }
        public int user_id { get; set; }
        public int is_deleted { get; set; }
        public int priority { get; set; }
        public int item_order { get; set; }
        public object responsible_uid { get; set; }
        public int project_id { get; set; }
        public int collapsed { get; set; }
        public string date_string { get; set; }

        //Added by me

        public override string ToString()
        {
            return content + "[" + id + "]";
        }

        public override bool Equals(object obj)
        {
            return (obj as Item)?.id == this.id;
        }


        public List<Label> labelList = new List<Label>();

    }

    public class TempIdMapping
    {
    }

    public class Reminder
    {
        public int is_deleted { get; set; }
        public string service { get; set; }
        public int id { get; set; }
        public string due_date_utc { get; set; }
        public int minute_offset { get; set; }
        public int item_id { get; set; }
        public int notify_uid { get; set; }
        public string type { get; set; }
        public string date_lang { get; set; }
    }

    

    public class SyncObject
    {
        public long seq_no_global { get; set; }
        public List<object> CollaboratorStates { get; set; }
        public List<object> ProjectNotes { get; set; }
        public string DayOrdersTimestamp { get; set; }
        public List<Note> Notes { get; set; }
        public List<Label> Labels { get; set; }
        public int UserId { get; set; }
        public List<object> Locations { get; set; }
        public List<object> Collaborators { get; set; }
        public List<LiveNotification> LiveNotifications { get; set; }
        public long seq_no { get; set; }
        public User User { get; set; }
        public List<Filter> Filters { get; set; }
        public List<Item> Items { get; set; }
        public TempIdMapping TempIdMapping { get; set; }
        public List<Reminder> Reminders { get; set; }
        public List<Project> Projects { get; set; }
        public long LiveNotificationsLastRead { get; set; }

        //Added by me

        public Project getProjectById(int id)
        {
            foreach (var proj in Projects)
            {
                if (id == proj.id)
                {
                    return proj;
                }
            }
            throw new Exception("Project Id not found. Was:" + id);
        }

        internal Label getLabelbyId(int lb)
        {
            foreach(Label label in Labels)
            {
                if(label.id == lb)
                {
                    return label;
                }
            }
            throw new Exception("id was not found");
        }

        public void SortProjects()
        {
            Projects.Sort((p1, p2) => p1.item_order.CompareTo(p2.item_order));
        }
    }

    public class Project
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public int is_deleted { get; set; }
        public int collapsed { get; set; }
        public int id { get; set; }
        public object archived_date { get; set; }
        public int item_order { get; set; }
        public int indent { get; set; }
        public int archived_timestamp { get; set; }
        public bool shared { get; set; }
        public int is_archived { get; set; }
        public bool? inbox_project { get; set; }
        

        //Added by me

        public List<Item> tasks = new List<Item>();
        public void AddItem(Item it)
        {
            tasks.Add(it);
        }

        public void OrderItems()
        {
            tasks.Sort((p1, p2) => p1.item_order.CompareTo(p2.item_order));
        }

        public Item nextTodo
        {
            get
            {
                if (tasks.Count == 0)
                {
                    return new Item(){content = ""};
                }

                Item returnTask = new Item { indent = -1 };

                foreach(Item task in tasks)
                {
                    if (task.@checked == 1)
                        continue;
                    if(task.indent > returnTask.indent)
                    {
                        returnTask = task;
                    }
                    else
                    {
                        return returnTask;
                    }
                }
                return returnTask;
            }
        }

        public override string ToString()
        {
            return name + "[" + id + "]";
        }
    }

    public class Datum
    {
        public string due_date { get; set; }
        public int? assigned_by_uid { get; set; }
        public int is_archived { get; set; }
        public List<object> labels { get; set; }
        public int? sync_id { get; set; }
        public int postpone_count { get; set; }
        public int in_history { get; set; }
        public int indent { get; set; }
        public int @checked { get; set; }
        public string date_added { get; set; }
        public string date_lang { get; set; }
        public int id { get; set; }
        public int priority { get; set; }
        public int complete_count { get; set; }
        public int user_id { get; set; }
        public int mm_offset { get; set; }
        public int is_deleted { get; set; }
        public string content { get; set; }
        public int item_order { get; set; }
        public object seq_no { get; set; }
        public object responsible_uid { get; set; }
        public int project_id { get; set; }
        public int collapsed { get; set; }
        public string date_string { get; set; }
        public int? day_order { get; set; }
    }

    public class RootObject
    {
        public string query { get; set; }
        public string type { get; set; }
        public List<Datum> data { get; set; }
    }
}