using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_Bot.JsonObject
{
    public class GroupUserInfo
    {
        public long group_id; //群号
        public long user_id;   //QQ 号
        public string nickname; //昵称
        public string card; //群名片／备注
        public string sex; //性别，male 或 female 或 unknown
        public int age;  //年龄
        public string area;  //地区
        public int join_time; //加群时间戳
        public int last_sent_time; //最后发言时间戳
        public string level; //成员等级
        public string role;  //角色，owner 或 admin 或 member
        public bool unfriendly;  //是否不良记录成员
        public string title; //专属头衔
        public int title_expire_time;  //专属头衔过期时间戳
        public bool card_changeable;  //是否允许修改群名片
    }
}
