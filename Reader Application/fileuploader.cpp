#include <iostream>
#include <string>
#include <algorithm>
#include <signal.h>
#include <stdio.h>
#include <cstdlib>
#include "ltkcpp.h"
#include "impinj_ltkcpp.h"
#include "time.h"
#include <sstream>
#include <fstream>
#include <ctime>
#include <chrono>
#include <thread>
#include <net/if.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <errno.h>
#include <fstream>
#include <bitset>
#include <iostream>
// #include <filesystem>
#include <future>
#include <dirent.h>
#include <array>
#include <cstdio>
#include <memory>
#include <stdexcept>
#include "rapidjson/document.h"

using namespace LLRP;
using namespace std;
using namespace std::chrono;
using namespace rapidjson;


string m_apiUrl = "";
string m_macAddress = "";
int m_heartBeatRate;

void loadConfig()
{
  std::ifstream in("./config.json");
  std::string contents((std::istreambuf_iterator<char>(in)), 
  std::istreambuf_iterator<char>());

  Document document;
  document.Parse(contents.c_str());

  m_apiUrl = "http://10.10.10.232/RFIDApplication/API/RFID";
  m_heartBeatRate = 10000;

  if(document.HasMember("ApiUrl"))
  {
    if(document["ApiUrl"].IsString())
    {
      cout << "Found the API URL: " << document["ApiUrl"].GetString() << endl;
      m_apiUrl = document["ApiUrl"].GetString();
    }
    else
    {
      cout << "API URL is not a string." << endl;
    }
  }
  else
  {
    cout << "No API URL was in the config file" << endl;
  }

  if(document.HasMember("HeartbeatRate"))
  {
    if(document["HeartbeatRate"].IsInt())
    {
      cout << "Found the Heart Beat Rate (ms): " << document["HeartbeatRate"].GetInt() << endl;
      m_heartBeatRate = document["HeartbeatRate"].GetInt();
    }
    else
    {
      cout << "Heart Beat Rate is not an integer." << endl;
    }
  }
  else
  {
    cout << "No Heart Beat Rate was in the config file" << endl;
  }
}

void getMacAddress()
{
  int fd;
  char  uc_Mac[32] = {0};
  
  struct ifreq ifr;
  char *iface = "eth0";
  char *mac;
  
  fd = socket(AF_INET, SOCK_DGRAM, 0);

  ifr.ifr_addr.sa_family = AF_INET;
  strncpy((char *)ifr.ifr_name , (const char *)iface , IFNAMSIZ-1);

  ioctl(fd, SIOCGIFHWADDR, &ifr);

  close(fd);
  
  mac = (char *)ifr.ifr_hwaddr.sa_data;
  
  //display mac address
  sprintf((char *)uc_Mac,(const char *)"%.2X:%.2X:%.2X:%.2X:%.2X:%.2X" , mac[0]&0xFF, mac[1]&0xFF, mac[2]&0xFF, mac[3]&0xFF, mac[4]&0xFF, mac[5]&0xFF);
  m_macAddress = string(uc_Mac);
}


string cmdExec(const char* cmd) {
    std::array<char, 128> buffer;
    std::string result;
    std::shared_ptr<FILE> pipe(popen(cmd, "r"), pclose);
    if (!pipe) throw std::runtime_error("popen() failed!");
    while (!feof(pipe.get())) {
        if (fgets(buffer.data(), 128, pipe.get()) != nullptr)
            result += buffer.data();
    }
    return result;
}

// Function to read directory and upload file data
int
main( int                           ac,
      char *                        av[])
{
  
  loadConfig();
  getMacAddress();

  string fileName;
  string commandBase = "curl -s -H \"Content-Type: application/json\" -X POST --data ";
  string commandEnd = " " + m_apiUrl;

  while(true)
  {
    if (auto dir = opendir(".")) 
    {
      bool uploaded = false;
      while (auto f = readdir(dir)) 
      {
        if (!f->d_name || f->d_name[0] == '.')
            continue; // Skip everything that starts with a dot

        fileName = f->d_name;
        if((fileName.find(".json") == std::string::npos) || (fileName.length() != 18))
          continue;

        try
        {
          int lastDot = strrchr(f->d_name, '.') - f->d_name;
          // cout << "File: " << fileName << endl;
          
          int64_t fileTime = stoll(fileName.substr(0, lastDot).c_str());
          int64_t tm = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
          int64_t age = tm - fileTime;
          // cout << "File age (ms): " << age << endl;

          if(age > 60000)
          {
            cout << "** Uploading file: " << fileName << endl;

            string command = commandBase + "@" + fileName + commandEnd;
            // cout << "**** Curling command: " << command << endl;
            string result = cmdExec(command.c_str());
            // cout << "**** Result: " << result << endl;
            
            uploaded = true;

            if(result.find("{\"status\":\"Success\"") != std::string::npos)
            {

              cout << "** Deleting file: " << fileName << endl;
              std::remove(fileName.c_str());
            }
            else
            {
              // cout << "Apparently this failed?" << endl;
            }
          }
        }
        catch (const std::exception& e)
        {
          // cout << "Threw an error, and I don't care..." << endl;
        }
        
      }
    
      closedir(dir);

      if(!uploaded)
      {
        cout << "** Uploading heartbeat ** " << endl;
        string command = commandBase + "'{\"readerId\":\"" + m_macAddress + "\", \"tagCount\":0}'" + commandEnd;
        // cout << "**** Curling command: " << command << endl;
        string result = cmdExec(command.c_str());
        // cout << "**** Result: " << result << endl;
      }
    }

    std::this_thread::sleep_for(std::chrono::milliseconds(m_heartBeatRate));
  }
}