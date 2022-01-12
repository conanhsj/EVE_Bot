using Bot.CoCRequest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.CoCRequest
{
    public static class ConstInfo
    {
        #region 属性描述
        public static List<DescriptionStatus> lstSTR = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 200, Result = "怪物之力" },
            new DescriptionStatus(){ Point = 140, Result = "超越人类之力" },
            new DescriptionStatus(){ Point = 99, Result = "世界水平，人类极限" },
            new DescriptionStatus(){ Point = 90, Result = "你见过的力气最大的人" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "弱者，手无缚鸡之力" },
            new DescriptionStatus(){ Point =  0, Result = "衰弱，几乎没法站起来" },
        };
        public static List<DescriptionStatus> lstCON = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 200, Result = "怪物之体,免疫大部分地球疾病" },
            new DescriptionStatus(){ Point = 140, Result = "超越人类之体格" },
            new DescriptionStatus(){ Point = 99, Result = "铁之刚体｡可以承受住最强的疼痛｡人类极限" },
            new DescriptionStatus(){ Point = 90, Result = "强者。抖落身上的液氮,强壮而精神" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "体弱多病。" },
            new DescriptionStatus(){ Point = 1, Result = "病弱｡卧床不起的人。" },
            new DescriptionStatus(){ Point = 0, Result = "死亡" },
        };
        public static List<DescriptionStatus> lstSIZ = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 200, Result = "1920磅/872kg" },
            new DescriptionStatus(){ Point = 180, Result = "记录中最重的人类(1400磅/634kg)" },
            new DescriptionStatus(){ Point = 150, Result = "马或牛(960磅/436kg)" },
            new DescriptionStatus(){ Point = 99, Result = "人类极限" },
            new DescriptionStatus(){ Point = 80, Result = "精壮成年人" },
            new DescriptionStatus(){ Point = 65, Result = "成年人" },
            new DescriptionStatus(){ Point = 50, Result = "瘦弱成年人" },
            new DescriptionStatus(){ Point = 30, Result = "初中生" },
            new DescriptionStatus(){ Point = 15, Result = "小学生" },
            new DescriptionStatus(){ Point =  1, Result = "婴儿" },
        };
        public static List<DescriptionStatus> lstDEX = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 200, Result = "闪电之速｡" },
            new DescriptionStatus(){ Point = 120, Result = "超越人类之速" },
            new DescriptionStatus(){ Point = 99, Result = "世界级运动员｡人类极限｡" },
            new DescriptionStatus(){ Point = 90, Result = "高速而灵活,可以达成超凡的技艺" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "缓慢,笨拙,无法行动自如" },
            new DescriptionStatus(){ Point =  0, Result = "没有协助无法移动" },
        };
        public static List<DescriptionStatus> lstAPP = new List<DescriptionStatus>() {
            new DescriptionStatus(){ Point = 99, Result = "魅力和酷的巅峰｡人类极限｡" },
            new DescriptionStatus(){ Point = 90, Result = "你见过的最漂亮的人" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "挫｡估计是因为事故或衰老｡" },
            new DescriptionStatus(){ Point =  0, Result = "如此的难看｡令人恐惧､厌恶和怜悯｡" },
        };
        public static List<DescriptionStatus> lstINT = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 200, Result = "怪物之智,可以理解并操作多重次元" },
            new DescriptionStatus(){ Point = 140, Result = "超越人类之智" },
            new DescriptionStatus(){ Point = 99, Result = "天才，人类极限" },
            new DescriptionStatus(){ Point = 90, Result = "超凡之脑,可以理解多门语言或法则" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "学得很慢,只能理解常用数字,学前教育级别的书｡" },
            new DescriptionStatus(){ Point =  0, Result = "没有智商,无法理解周遭的世界" },
        };
        public static List<DescriptionStatus> lstPOW = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 210, Result = "怪物潜质，超越凡人之理解力" },
            new DescriptionStatus(){ Point = 140, Result = "超越人类,基本上是异界存在" },
            new DescriptionStatus(){ Point = 100, Result = "钢铁之心" },
            new DescriptionStatus(){ Point = 90, Result = "坚强的心" },
            new DescriptionStatus(){ Point = 50, Result = "普通人水平" },
            new DescriptionStatus(){ Point = 15, Result = "意志力弱" },
            new DescriptionStatus(){ Point =  0, Result = "弱者的心" },
        };
        public static List<DescriptionStatus> lstEDU = new List<DescriptionStatus>() {

            new DescriptionStatus(){ Point = 99, Result = "人类极限" },
            new DescriptionStatus(){ Point = 96, Result = "世界级权威" },
            new DescriptionStatus(){ Point = 90, Result = "博士学位,教授" },
            new DescriptionStatus(){ Point = 80, Result = "研究生毕业" },
            new DescriptionStatus(){ Point = 70, Result = "大学毕业" },
            new DescriptionStatus(){ Point = 70, Result = "高中毕业" },
            new DescriptionStatus(){ Point = 15, Result = "没有受过教育" },
            new DescriptionStatus(){ Point =  0, Result = "新生儿" },
        };
        #endregion

        //思想/信念
        public static List<string> lstBelief = new List<string>()
        {
            "你信仰并祈并一位大能。（例如毗沙门天、耶稣基督、海尔·塞拉西一世）",
            "人类无需上帝。（例如坚定的无神论者，人文主义者，世俗主义者）",
            "科学万能！科学万岁！你将选择其中之一。（例如进化论，低温学，太空探索）",
            "命中注定。（例如因果报应，种姓系统，超自然存在）",
            "社团或秘密结社的一员。（例如共济会，女协，匿名者）",
            "社会坏掉了，而你将成为正义的伙伴。应斩除之物是？（例如毒品，暴力，种族歧视）",
            "神秘依然在。（例如占星术，招魂术，塔罗）",
            "诸君，我喜欢政治。（例如保守党，共产党，自由党）",
            "“金钱就是力量，我的朋友，我将竭尽全力获取我能看到的一切。”（例如贪婪心，进取心，冷酷心）",
            "竞选者/激进主义者。（例如女权运动人，平等主义家，工会权柄）",
        };
        //重要之人
        public static List<string> lstVIP = new List<string>()
        {
            "父辈。（例如母亲，父亲，继母）",
            "祖父辈。（例如外祖母，祖父）",
            "兄弟。（例如妹妹，半血亲妹妹，无血缘妹妹）",
            "孩子。（儿子或女儿）",
            "另一半。（例如配偶，未婚夫，爱人）",
            "那位指引你人生技能的人。指明该技能和该人。（例如学校教师，师傅，父亲）",
            "青梅竹马。（例如同学，邻居，幼驯染）",
            "名人。偶像或者英雄。当然也许你从未见过他。（例如电影明星，政治家，音乐家。）",
            "游戏中的另一位调查员伙伴。随机或自选。",
            "游戏中另一外ＮＰＣ。详情咨询你的守秘人。",
        };
        //原因
        public static List<string> lstReason = new List<string>()
        {
            "你欠了他们人情。他们帮助了你什么？（例如，经济上，困难时期的庇护，给你第一份工作）",
            "他们教会了你一些东西。（例如，技能，如何去爱，如何成为男子汉）",
            "他们给了你生命的意义。（例如，你渴望成为他们那样的人，你苦苦追寻着他们，你想让他们高兴）",
            "你曾害了他们，而现在寻求救赎。例如，偷窃了他们的钱财，向警方报告了他们的行踪，在他们绝望时拒绝救助）",
            "同甘共苦。（例如，你们共同经历过困难时期，你们携手成长，共同度过战争）",
            "你想向他们证明自己。（例如，自己找到工作，自己搞到老婆，自己考到学历）",
            "你崇拜他们。（例如，崇拜他们的名头，他们的魅力，他们的工作）",
            "后悔的感觉。（例如，你本应死在他们面前，你背弃了你的誓言，你在可以助人之时驻足不前）",
            "你试图证明你比他们更出色。他们的缺点是？（例如，懒惰，酗酒，冷漠）",
            "他们扰乱了你的人生，而你寻求复仇。发生了什么？（例如，射杀爱人之日，国破家亡之时，明镜两分之际）",
        };
        //重要之地
        public static List<string> lstPlace = new List<string>()
        {
            "你最爱的学府。（例如，中学，大学）",
            "你的故乡。（例如，乡下老家，小镇村，大都市）",
            "相识初恋之处。（例如，音乐会，度假村，核弹避难所）",
            "静思之地。（例如，图书馆，你的乡土别墅，钓鱼中）",
            "社交之地。（例如，绅士俱乐部，地方酒吧，叔叔的家）",
            "联系你思想/信念的场所。（例如，小教堂，麦加，巨石阵）",
            "重要之人的坟墓。（例如，另一半，孩子，爱人）",
            "家族所在。（例如，乡下小屋，租屋，幼年的孤儿院）",
            "生命中最高兴时的所在。（例如，初吻时坐着的公园长椅，你的大学）",
            "工作地点。（例如，办公室，图书馆，银行）",
        };
        //宝贵之物
        public static List<string> lstTreasure = new List<string>()
        {
            "与你得意技相关之物。（例如华服，假ＩＤ卡，青铜指虎）",
            "职业必需品。（例如医疗包，汽车，撬锁器）",
            "童年的遗留物。（例如漫画书，随身小刀，幸运币）",
            "逝者遗物。（例如烛堡，钱包里的遗照，信）",
            "重要之人给予之物。（例如戒指，日志，地图）",
            "收藏品。（例如撤票，标本，记录）",
            "你发掘而不知真相的东西。答案追寻中。（例如，橱柜里找到的未知语言信件，一根奇怪的从父亲出继承来的来源不明的风琴，花园里挖出来的奇妙的银球）",
            "体育用品。（例如，球棒，签名棒球，鱼竿）",
            "武器。（例如，半自动左轮，老旧的猎用来福，靴刃）",
            "宠物。（例如狗，猫，乌龟）",
        };
        //特质
        public static List<string> lstSpecial = new List<string>()
        {
            "慷慨大方。（例如，小费大手，及时雨，慈善家）",
            "善待动物。（例如，爱猫人士，农场出生，与马同舞）",
            "梦想家。（例如，惯常异想天开，预言家，创造者）",
            "享乐主义者。（例如，派对大师，酒吧醉汉，“放纵到死”）",
            "赌徒，冒险家。（例如，扑克脸，任何事都来一遍，活在生死边缘）",
            "好厨子，好吃货。（例如，烤得一手好蛋糕，无米之炊都能做好，优雅的食神）",
            "女人缘/万人迷。（例如，长袖善舞，甜言蜜语，电眼乱放）",
            "忠心在我。（例如，背负自己的朋友，从未破誓，为信念而死）",
            "好名头。（例如，村里最好的饭后聊天人士，虔信圣徒，不惧任何危险）",
            "雄心壮志。（例如，梦想远大，目标是成为ＢＯＳＳ，渴求一切）",
        };


        public static List<Job> lstJobs = new List<Job>()
        {
            new Job(){ Name="古文物学家/古董收藏家【原作向】",
                       Description ="估价，艺术与手艺（任一），历史，图书馆使用，其他语言，社交技能（魅惑、话术、恐吓或说服）其一，侦查，自选一技能",
                       CreditMin = 30, CreditMax = 70, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="艺术家",
                       Description ="艺术与手艺（任一），（博物学、历史）其一，社交技能（魅惑、话术、恐吓或说服）其一，其他语言，心理学，侦查，自选二技能",
                       CreditMin = 9, CreditMax = 50, SkillPointMemo = "教育ｘ２＋意志ｘ２或敏捷ｘ２"},
            new Job(){ Name="运动员",
                       Description ="攀爬，跳跃，格斗（拳击），骑术，社交技能（魅惑、话术、恐吓或说服）其一，游泳，投掷，自选一技能",
                       CreditMin = 9, CreditMax = 70, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="作家【原作向】",
                       Description ="艺术（文学），历史，图书馆使用，（博物学、神秘学）其一，其他语言，母语，心理学，自选一技能",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="神职人员",
                       Description ="会计学，历史，图书馆使用，聆听，其他语言，社交技能（魅惑、话术、恐吓或说服）其一，心理学，自选一技能",
                       CreditMin = 9, CreditMax = 60, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="罪犯",
                       Description ="社交技能（魅惑、话术、恐吓或说服）其一，心理学，侦查，潜行，（估价、易容、格斗、射击、锁匠、机械维修、妙手）其四",
                       CreditMin = 5, CreditMax = 65, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="业余艺术爱好者【原作向】",
                       Description ="艺术与手艺（任一），射击，其他语言，骑术，社交技能（魅惑、话术、恐吓或说服）其一，自选三技能",
                       CreditMin = 50, CreditMax = 99, SkillPointMemo = "教育ｘ２＋外貌ｘ２"},
            new Job(){ Name="医生【原作向】",
                       Description ="急救，其他语言（拉丁文），医学，心理学，科学（生物学），科学（药学），任两种有关学术或个人专业的技能（例如精神医生选择精神分析）",
                       CreditMin = 30, CreditMax = 80, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="流浪者",
                       Description ="攀爬，跳跃，聆听，领航，社交技能（魅惑、话术、恐吓或说服）其一，潜行，自选二技能",
                       CreditMin = 0, CreditMax = 5, SkillPointMemo = "教育ｘ２＋外貌ｘ２或敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="工程师",
                       Description ="艺术与手艺（设计图纸），电气维修，图书馆使用，机械维修，操作重型机械，科学（工程学），科学（物理学），自选一技能",
                       CreditMin = 30, CreditMax = 60, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="艺人/演艺人员",
                       Description ="艺术与手艺（表演），乔装，社交技能（魅惑、话术、恐吓或说服）其二，聆听，心理学，自选二技能",
                       CreditMin = 9, CreditMax = 70, SkillPointMemo = "教育ｘ２＋外貌ｘ２"},
            new Job(){ Name="农民",
                       Description ="艺术与手艺（农事），汽车（或畜车）驾驶，社交技能（魅惑、话术、恐吓或说服）其一，机械维修，博物学，操作重型机械，追踪，自选一技能",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="黑客【现代】",
                       Description ="计算机使用，电气维修，电子学，图书馆使用，侦查，社交技能（魅惑、话术、恐吓或说服）其一，自选二技能",
                       CreditMin = 10, CreditMax = 70, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="记者【原作向】",
                       Description ="艺术与手艺（摄影），历史，图书馆使用，其他语言，社交技能（魅惑、话术、恐吓或说服）其一，心理学，自选二技能",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="律师",
                       Description ="会计学，法律，图书馆使用，社交技能（魅惑、话术、恐吓或说服）其二，心理学，自选二技能",
                       CreditMin = 30, CreditMax = 80, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="图书馆管理员【原作向】",
                       Description ="会计学，图书馆使用，其他语言，母语，任意四个与个人特质和阅读专业有关的技能",
                       CreditMin = 9, CreditMax = 35, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="军官",
                       Description ="会计学，射击，领航，社交技能（魅惑、话术、恐吓或说服）其二，心理学，生存，自选一技能",
                       CreditMin = 20, CreditMax = 70, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="传教士",
                       Description ="艺术与手艺（任一），机械维修，医学，博物学，社交技能（魅惑、话术、恐吓或说服）其一，自选二技能",
                       CreditMin = 0, CreditMax = 30, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="音乐家",
                       Description ="艺术与手艺（乐器），一种社交技能（魅惑、话术、恐吓或说服），聆听，心理学，自选四技能",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或意志ｘ２"},
            new Job(){ Name="超心理学家",
                       Description ="人类学，艺术与手艺（摄影），历史，图书馆使用，神秘学，其他语言，心理学，自选一技能",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="飞行员",
                       Description ="电气维修，机械维修，领航，操作重型机械，驾驶（飞行器），科学（天文学），自选二技能",
                       CreditMin = 20, CreditMax = 70, SkillPointMemo = "教育ｘ２＋敏捷ｘ２"},
            new Job(){ Name="警探【原作向】",
                       Description ="艺术与手艺（表演）或乔装，射击，法律，聆听，一种社交技能（魅惑、话术、恐吓或说服），心理学，侦查，自选一技能",
                       CreditMin = 20, CreditMax = 50, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="警察",
                       Description ="—格斗（拳击），射击，急救，一种社交技能（魅惑、话术、恐吓或说服），法律，心理学，侦查，下列根据个人选一：汽车驾驶或骑术",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="私家侦探",
                       Description ="艺术与手艺（摄影），乔装，法律，图书馆使用，一种社交技能（魅惑、话术、恐吓或说服），心理学，侦查，自选一技能（例如计算机使用、锁匠、射击）",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="教授【原作向】",
                       Description ="图书馆使用，其他语言，母语，心理学，自选四种与学术或个人专业有关的技能",
                       CreditMin = 20, CreditMax = 70, SkillPointMemo = "教育ｘ４"},
            new Job(){ Name="士兵",
                       Description ="攀爬或游泳，闪避，格斗，射击，潜行，生存，下列选二：急救、机械维修、其他语言",
                       CreditMin = 9, CreditMax = 30, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="部落成员",
                       Description ="攀爬，格斗或投掷，博物学，聆听，神秘学，侦查，游泳，生存（任一）",
                       CreditMin = 0, CreditMax = 15, SkillPointMemo = "教育ｘ２＋敏捷ｘ２或力量ｘ２"},
            new Job(){ Name="狂热者",
                       Description ="历史，二种社交技能（魅惑、话术、恐吓或说服），心理学，潜行，自选三技能",
                       CreditMin = 0, CreditMax = 30, SkillPointMemo = "教育ｘ２＋外貌ｘ２或意志ｘ２"},
        };
        public static List<Skill> lstSkills = new List<Skill>()
        {
            new Skill(){ Name ="会计", BasePoint = 5},
            new Skill(){ Name ="动物驯养", BasePoint = 5, Comment="非常规技能"},
            new Skill(){ Name ="人类学", BasePoint = 1},
            new Skill(){ Name ="估价", BasePoint = 5},
            new Skill(){ Name ="考古学", BasePoint = 5},
            new Skill(){ Name ="魅惑", BasePoint = 15},
            new Skill(){ Name ="攀爬", BasePoint = 20},
            new Skill(){ Name ="乔装", BasePoint = 5},
            new Skill(){ Name ="法律", BasePoint = 5},
            new Skill(){ Name ="图书馆使用", BasePoint = 20},
            new Skill(){ Name ="聆听", BasePoint = 20},
            new Skill(){ Name ="锁匠", BasePoint = 1},
            new Skill(){ Name ="汽车驾驶", BasePoint = 20},
            new Skill(){ Name ="电气维修", BasePoint = 10},
            new Skill(){ Name ="话术", BasePoint = 5},
            new Skill(){ Name ="急救", BasePoint = 30},
            new Skill(){ Name ="历史", BasePoint = 5},
            new Skill(){ Name ="恐吓", BasePoint = 15},
            new Skill(){ Name ="跳跃", BasePoint = 20},
            new Skill(){ Name ="博物学", BasePoint = 10},
            new Skill(){ Name ="领航", BasePoint = 10},
            new Skill(){ Name ="神秘学", BasePoint = 5},
            new Skill(){ Name ="操作重型机械", BasePoint = 1},
            new Skill(){ Name ="说服", BasePoint = 10},
            new Skill(){ Name ="机械维修", BasePoint = 10},
            new Skill(){ Name ="信用评级", BasePoint = 0},
            new Skill(){ Name ="克苏鲁神话", BasePoint = 0},
            new Skill(){ Name ="游泳", BasePoint = 20},
            new Skill(){ Name ="投掷", BasePoint = 20},
            new Skill(){ Name ="追踪", BasePoint = 10},
            new Skill(){ Name ="骑术", BasePoint = 5},
            new Skill(){ Name ="医学", BasePoint = 1},
            new Skill(){ Name ="精神分析", BasePoint = 1},
            new Skill(){ Name ="心理学", BasePoint = 10},
            new Skill(){ Name ="妙手", BasePoint = 10},
            new Skill(){ Name ="侦查", BasePoint = 25},
            new Skill(){ Name ="潜行", BasePoint = 20},
            new Skill(){ Name ="炮术", BasePoint = 1, Comment="非常规技能"},
            new Skill(){ Name ="爆破", BasePoint = 1, Comment="非常规技能"},
            new Skill(){ Name ="潜水", BasePoint = 1, Comment="非常规技能"},
            new Skill(){ Name ="催眠", BasePoint = 1, Comment="非常规技能"},
            new Skill(){ Name ="读唇", BasePoint = 1, Comment="非常规技能"},
            new Skill(){ Name ="学问", BasePoint = 1, Comment="非常规技能，多个不同的独立技能"},
            new Skill(){ Name ="驾驶", BasePoint = 1, Comment="多个不同的独立技能"},
            new Skill(){ Name ="生存", BasePoint = 10, Comment="多个不同的独立技能"},
            new Skill(){ Name ="计算机使用", BasePoint = 5, Comment="只能用于现代游戏中"},

            new Skill(){ Name ="艺术与手艺", BasePoint = 5, Comment="多个不同的独立技能"},
            new Skill(){ Name ="表演", BasePoint = 5, Comment="见艺术与手艺"},
            new Skill(){ Name ="美术", BasePoint = 5, Comment="见艺术与手艺"},
            new Skill(){ Name ="伪造", BasePoint = 5, Comment="见艺术与手艺"},
            new Skill(){ Name ="摄影", BasePoint = 5, Comment="见艺术与手艺"},

            new Skill(){ Name ="科学", BasePoint = 1, Comment="多个不同的独立技能"},
            new Skill(){ Name ="天文学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="生物学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="植物学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="化学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="密码学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="工程学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="司法科学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="地质学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="数学", BasePoint = 10, Comment="见科学"},
            new Skill(){ Name ="动物学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="气象学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="药学", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="物理", BasePoint = 1, Comment="见科学"},
            new Skill(){ Name ="电子学", BasePoint = 1, Comment="只能用于现代游戏中"},

            new Skill(){ Name ="格斗", BasePoint = 0, Comment="多个不同的独立技能"},
            new Skill(){ Name ="斧", BasePoint = 15, Comment="见格斗"},
            new Skill(){ Name ="斗殴", BasePoint = 25, Comment="见格斗"},
            new Skill(){ Name ="链锯", BasePoint = 10, Comment="见格斗"},
            new Skill(){ Name ="连枷", BasePoint = 10, Comment="见格斗"},
            new Skill(){ Name ="绞索", BasePoint = 15, Comment="见格斗"},
            new Skill(){ Name ="剑", BasePoint = 20, Comment="见格斗"},
            new Skill(){ Name ="鞭", BasePoint = 10, Comment="见格斗"},

            new Skill(){ Name ="射击", BasePoint = 0, Comment="多个不同的独立技能"},
            new Skill(){ Name ="弓", BasePoint = 15, Comment="见射击"},
            new Skill(){ Name ="火焰喷射器", BasePoint = 10, Comment="见射击"},
            new Skill(){ Name ="机枪", BasePoint = 10, Comment="见射击"},
            new Skill(){ Name ="手枪", BasePoint = 20, Comment="见射击"},
            new Skill(){ Name ="重武器", BasePoint = 10, Comment="见射击"},
            new Skill(){ Name ="冲锋枪", BasePoint = 15, Comment="见射击"},
            new Skill(){ Name ="步枪", BasePoint = 25, Comment="见射击/步霰"},
            new Skill(){ Name ="霰弹枪", BasePoint = 25, Comment="见射击/步霰"},
            new Skill(){ Name ="矛", BasePoint = 20, Comment="见射击(或投掷)"},


            new Skill(){ Name ="闪避", BasePoint = 0, Comment="为敏捷值一半"},

            new Skill(){ Name ="语言（其他）", BasePoint = 1, Comment="多个不同的独立技能"},
            new Skill(){ Name ="语言（母语）", BasePoint = 0, Comment="为教育值"},

        };

    }
}
