using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardSQL
{
    internal class StudySessionService
    {
        //this variable is used to store the connection string
        private static string studySessionConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //this method will record the study session
        public void RecordStudySession(int stackId, int score)
        {
            using (var connection = new SqlConnection(studySessionConnectionString))
            {
                connection.Open();

                // Insert a new study session record
                string insertQuery = "INSERT INTO StudySessions (StackId, Date, Score) VALUES (@StackId, @Date, @Score)";
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@StackId", stackId);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
                    command.Parameters.AddWithValue("@Score", score);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<StudySession> GetStudySessions()
        {
            List<StudySession> studySessions = new List<StudySession>();

            using (var connection = new SqlConnection(studySessionConnectionString))
            {
                connection.Open();

                // Retrieve study sessions
                string selectQuery = "SELECT StudySessionId, StackId, Date, Score FROM StudySessions";
                using (var command = new SqlCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudySession studySession = new StudySession
                            {
                                StudySessionId = Convert.ToInt32(reader["StudySessionId"]),
                                StackId = Convert.ToInt32(reader["StackId"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Score = Convert.ToInt32(reader["Score"])
                            };

                            studySessions.Add(studySession);
                        }
                    }
                }
            }

            return studySessions;
        }
    }
}
