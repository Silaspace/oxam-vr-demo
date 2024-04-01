#!/bin/sh

URL="https://query1.finance.yahoo.com/v7/finance/download/ROYMY?period1=1680256514&period2=1711878914&interval=1d&events=history&includeAdjustedClose=true"

mkdir data
curl -L $URL -o data/price.csv