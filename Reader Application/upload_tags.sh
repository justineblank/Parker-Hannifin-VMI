#!/bin/bash

url=10.10.10.232
data="'`cat 1520821095764.json`'"

echo "Data: "
echo $data
echo 

wget  \
  --method POST \
  --header "'content-type: application/json'" \
  --body-data ${data} \
  --output-document \
  - http://$url/RFIDApplication/API/RFID
