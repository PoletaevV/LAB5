using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TestPr
{
    class Program
    {
        static void Main(string[] args)
        {
            //1 part
            ResearchTeam RTeam1 = new ResearchTeam("Topic", "MIET", 123, TimeFrame.Year);
            Person person1 = new Person("Ivan", "Ivanov", new DateTime(2000, 3, 3));
            Person person2 = new Person("Boris", "Borisov", new DateTime(1984, 11, 5));
            Paper paper1 = new Paper("Paper1", person1, new DateTime(2016, 4, 9));
            RTeam1.AddParticipants(person1, person2);
            RTeam1.AddPapers(paper1);
            ResearchTeam RTeam2 = RTeam1.DeepCopy();

            Console.WriteLine(RTeam1);
            Console.WriteLine(RTeam2);
            //2 part
            Console.WriteLine("Enter filename");
            string filename;
            filename = Console.ReadLine();
            if (!File.Exists(filename))
            {
                Console.WriteLine("File not exists.\nNew file must create");
                File.Create(filename).Close();
                Console.WriteLine("New file created");

            }
            else
            {
                if (RTeam1.Load(filename))
                    Console.WriteLine("File loaded");
                else
                    Console.WriteLine("File cant loaded");
            }
            //3 part

            Console.WriteLine(RTeam1);
            //4 part
            RTeam1.AddFromConsole();
            if(RTeam1.Save(filename))
                Console.WriteLine("File saved");
            else
                Console.WriteLine("File not saved");
            Console.WriteLine(RTeam1);

            //5 part
            if (ResearchTeam.Load(filename, RTeam1))
                Console.WriteLine("File loaded");
            else
                Console.WriteLine("File cant loaded");
            RTeam1.AddFromConsole();
            if (ResearchTeam.Save(filename, RTeam1))
                Console.WriteLine("File saved");
            else
                Console.WriteLine("File cant saved");
            //6 part
            Console.WriteLine(RTeam1);
            
        }
    }
}
