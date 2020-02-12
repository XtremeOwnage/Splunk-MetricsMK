# Splunk-MetricsMK

## What is this?

This repository is dedicated to automating the conversion of Splunk Perfmon inputs to instead use Metric MultiKey/Value data. By doing so, we can reduce license usage by up to 98%, while having must faster searches and higher efficiency.

## How do I use this?

Well- its not finished. But, you can drop the CreatePropsTransforms.py into your app/local folder (containing your inputs.conf), and execute it inplace. It should generate a new props.conf and transforms.conf allowing the data to be ingested as metrics.

## What needs to be done?

See https://github.com/XtremeOwnageDotCom/Splunk-MetricsMK/issues
