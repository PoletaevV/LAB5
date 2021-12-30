using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestPr
{
    public enum Revision { Remove, Replace, Property };
    delegate void ResearchTeamsChangedHandler<TKey>(object source, ResearchTeamsChangedEventArgs<TKey> args);
    //delegate void PropertyChangedEventHandler(Object sender, PropertyChangedEventArgs e);
    [Serializable]
    class ResearchTeam : Team, INameAndCopy, INotifyPropertyChanged
    {
        private string resTopic;
        private TimeFrame duration;
        private List<Person> pubParticipants = new List<Person>();
        private List<Paper> pubList = new List<Paper>();

        public event PropertyChangedEventHandler PropertyChanged;
        //private Team pubTeam = new Team();
        //constructor with parameters
        public ResearchTeam(string resTopicValue, string orgNameValue, int regNumberValue, TimeFrame durationValue)
        {
            resTopic = resTopicValue;
            OrgName = orgNameValue;
            RegNumber = regNumberValue;
            duration = durationValue;
        }

        //constructor without parameters
        public ResearchTeam() : this("20th century economy", "MIET", 1234, TimeFrame.Year)
        {
            orgName = "MMM";
            regNumber = 111;
        }


        //properties
        public string ResTopic
        {
            get
            {
                return resTopic;
            }
            set
            {
                resTopic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResTopic"));
            }
        }



        public TimeFrame Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duration"));
            }
        }

        public List<Paper> PubList
        {
            get
            {
                return pubList;
            }
            set
            {
                pubList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PubList"));
            }
        }

        public List<Person> PubParticipants
        {
            get
            {
                return pubParticipants;
            }
            set
            {
                pubParticipants = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PubParticipants"));
            }
        }

        //property returning the latest publication
        public Paper LastPub
        {
            get
            {
                if ((PubList == null) || (PubList.Count == 0))
                    return null;
                else
                {
                    Paper tempPaper = PubList[0];
                    DateTime datMax = tempPaper.PubDate;
                    int indexMax = 0;
                    for (int i = 1; i < PubList.Count; i++)
                    {
                        tempPaper = pubList[i];
                        if (datMax < tempPaper.PubDate)
                        {
                            datMax = tempPaper.PubDate;
                            indexMax = i;
                        }
                    }
                    return PubList[indexMax];
                }
            }
        }


        //indexer
        public bool this[TimeFrame tFrame]
        {
            get
            {
                return Duration == tFrame;
            }
        }
        //pushback
        public void AddPapers(params Paper[] papers)
        {
            pubList.AddRange(papers);
            /*for (int i = 0; i < papers.Length; i++)
                pubList.Add(papers[i]);*/
        }

        public void AddParticipants(params Person[] persons)
        {
            pubParticipants.AddRange(persons);
            /*for(int i = 0; i < persons.Length; i++)
                pubParticipants.Add(persons[i]);*/
        }
        //outputs
        public override string ToString()
        {
            string strPubList = "";
            if (PubList.Count != 0)
                for (int i = 0; i < PubList.Count; i++)
                    strPubList += PubList[i].ToString() + " ";

            string strPubParticipants = "";
            if (PubParticipants.Count != 0)
                for (int i = 0; i < PubParticipants.Count; i++)
                    strPubParticipants += PubParticipants[i].ToString() + " ";

            return "Topic: " + ResTopic + "\nOrg name: " + OrgName + "\nTeam reg number: " + RegNumber.ToString() + "\nDuration: " + Duration.ToString() + " \n Papers:" + strPubList + "\n  Participants:" + strPubParticipants;
        }
        public virtual string ToShortString()
        {
            return ResTopic + " " + OrgName + " " + RegNumber.ToString() + " " + Duration.ToString();
        }
        /*public override object DeepCopy()
        {
            ResearchTeam newResTeam = new ResearchTeam(this.resTopic, this.orgName, this.regNumber, this.duration);
            newResTeam.pubList = new List<Paper>();
            newResTeam.pubParticipants = new List<Person>();
            for(int i=0;i<this.pubList.Count;i++)
            {
                //Paper tempPaper = this.pubList[i] as Paper;
                newResTeam.AddPapers(pubList[i].DeepCopy());
            }
            for (int i = 0; i < this.pubParticipants.Count; i++)
            {
                //Person tempPart = this.pubParticipants[i] as Person;
                newResTeam.AddParticipants(pubParticipants[i].DeepCopy());
            }
            return newResTeam;
        }*/
        public Team team
        {
            get
            {
                return new Team(orgName, regNumber);
            }
            set
            {
                orgName = value.OrgName;
                regNumber = value.RegNumber;
            }
        }
        string INameAndCopy.Name
        {
            get { return this.orgName; }
            set { this.orgName = value; }
        }
        /*
        object INameAndCopy.DeepCopy()
        {
            return this.DeepCopy();
        }
        */

        public IEnumerator GetEnumerator()
        {

            return new ResearchTeamEnumerator(pubParticipants, pubList);
        }

        public IEnumerable GetWithotPub()
        {
            int count = 0;
            for (int j = 0; j < pubParticipants.Count; j++)
            {
                count = 0;
                for (int i = 0; i < PubList.Count; i++)
                {
                    Paper paper = PubList[i] as Paper;
                    if (paper.PubAuthor.Equals(pubParticipants[j]))
                        count++;
                }
                if (count == 0)
                    yield return pubParticipants[j];

            }
        }

        public IEnumerable GetLastPub(int n)
        {
            DateTime today = DateTime.Today;
            for (int i = 0; i < pubList.Count; i++)
            {
                Paper paper = PubList[i] as Paper;
                TimeSpan dif = today - paper.PubDate;
                if (dif.Days / 365 <= n)
                    yield return paper;
            }
        }

        public IEnumerable More1Pub()
        {
            int count = 0;
            for (int j = 0; j < pubParticipants.Count; j++)
            {
                count = 0;
                for (int i = 0; i < PubList.Count; i++)
                {
                    Paper pap = PubList[i] as Paper;
                    if (pap.PubAuthor.Equals(pubParticipants[j]))
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    yield return pubParticipants[j];
                }
            }
        }
        public IEnumerable PapersInLastYear()
        {
            DateTime today = DateTime.Today;

            for (int i = 0; i < pubList.Count; i++)
            {
                Paper paper = pubList[i] as Paper;
                TimeSpan diff = today - paper.PubDate;
                if (diff.Days / 365 <= 1)
                {
                    yield return paper;
                }
            }
        }


        public void SortPapersDate()
        {
            pubList.Sort();
        }

        public void SortPapersTitle()
        {
            pubList.Sort(new Paper());
        }

        public void SortPapersAuthor()
        {
            pubList.Sort(new CompareAuthorsPap());
        }
        public ResearchTeam DeepCopy()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (ResearchTeam)formatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                stream.Close();

            }
            return null;
        }

        public bool Save(string filename)
        {

            FileStream SaveFileStream = new FileStream(filename, FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(SaveFileStream, this);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                SaveFileStream.Close();
            }
        }

        public bool Load(string filename)
        {
            FileStream LoadFileStream = new FileStream(filename, FileMode.Open);
            try
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                ResearchTeam temp = (ResearchTeam)deserializer.Deserialize(LoadFileStream);
                resTopic = temp.resTopic;
                OrgName = temp.orgName;
                RegNumber = temp.RegNumber;
                duration = temp.duration;
                pubParticipants = new List<Person>(temp.pubParticipants);
                pubList = new List<Paper>(temp.pubList);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                LoadFileStream.Close();
            }
        }

        public bool AddFromConsole()
        {
            Console.WriteLine("Enter string\nSeparators:'/',',','.'\nFormat: PubTitle/PesonName/PersonSurname/BirthdayYear/BirthdayMonth/BirthdayDay/pubDateYear/pubDateMonth/pubDateDay");

            //str = Console.ReadLine();
            string[] strArr = Console.ReadLine().Split('/', ',', '.');

            if (strArr.Length != 9)
            {
                Console.WriteLine("Invalid data");
                return false;
            }
            try
            {
                Paper temp = new Paper(strArr[0], new Person(strArr[1], strArr[2], new DateTime(int.Parse(strArr[3]), int.Parse(strArr[4]), int.Parse(strArr[5]))), new DateTime(int.Parse(strArr[6]), int.Parse(strArr[7]), int.Parse(strArr[8])));
                this.AddPapers(temp);
                return true;
            }
            catch
            {
                Console.WriteLine("Invalid data");
                return false;
            }
        }

        public static bool Save(string filename,ResearchTeam obj)
        {
            FileStream SaveFileStream = new FileStream(filename, FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(SaveFileStream, obj);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                SaveFileStream.Close();
            }
        }

        public static bool Load(string filename,ResearchTeam obj)
        {
            FileStream LoadFileStream = new FileStream(filename, FileMode.Open);
            try
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                ResearchTeam temp = (ResearchTeam)deserializer.Deserialize(LoadFileStream);
                obj.resTopic = temp.resTopic;
                obj.OrgName = temp.orgName;
                obj.RegNumber = temp.RegNumber;
                obj.duration = temp.duration;
                obj.pubParticipants = new List<Person>(temp.pubParticipants);
                obj.pubList = new List<Paper>(temp.pubList);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                LoadFileStream.Close();
            }
        }

    }
}
