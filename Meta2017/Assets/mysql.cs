using UnityEngine;
using MySql.Data.MySqlClient;

public class mysql : MonoBehaviour {

    public string Schema;
    public string Table;

	// Use this for initialization
	void Start () {
        string connStr = "server=localhost;user=admin;database=gamification;port=3306;password=admin";
        Debug.Log("Before Connection");
        MySqlConnection conn = new MySqlConnection(connStr);
        Debug.Log("After Connection");
        try
        {
            Debug.Log("Connecting to MySQL...");
            conn.Open();

            string sql = "SELECT * FROM gamification.users";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Debug.Log(rdr[0] + " -- " + rdr[1]);
            }
            
            rdr.Close();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        conn.Close();
        Debug.Log("Done.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
