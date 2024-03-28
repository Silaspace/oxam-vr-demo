#!/bin/bash

URL='https://lobsterdata.com/info/sample/LOBSTER_SampleFile_AMZN_2012-06-21_1.zip'
curl "$URL" --output data.zip
unzip data.zip -d ./data

rm data.zip
