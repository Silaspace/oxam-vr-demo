#!/bin/sh

URL_PCA_2D="https://gist.github.com/ollybritton/df3bf7d80dbaf18db6fa59cb77eec55f/raw/f4a7544e24c0cbcf9b7741a99cc62c0779df65df/pca_2d.csv"
URL_PCA_3D="https://gist.github.com/ollybritton/df3bf7d80dbaf18db6fa59cb77eec55f/raw/f4a7544e24c0cbcf9b7741a99cc62c0779df65df/pca_3d.csv"

mkdir data

curl -L $URL_PCA_2D -o data/pca_2d.csv
curl -L $URL_PCA_3D -o data/pca_3d.csv