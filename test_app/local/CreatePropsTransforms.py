import configparser


#region Load configuration files
file_inputs_conf = configparser.ConfigParser(strict=False, interpolation=None)
file_inputs_conf.read('inputs.conf')
file_props_conf.optionxform = str
file_transforms_conf = configparser.ConfigParser(strict=False, interpolation=None)
file_transforms_conf.optionxform = str
file_transforms_conf.read('transforms.conf')
file_props_conf = configparser.ConfigParser(strict=False, interpolation=None)
file_props_conf.optionxform = str
file_props_conf.read('props.conf')
#endregion

for input_stanza in file_inputs_conf.sections():
    if(input_stanza.endswith("_MVMetric") and file_inputs_conf[input_stanza].get('mode', 'false') == 'multikv'):
        cfg_input = file_inputs_conf[input_stanza]
        inputname = cfg_input['object']
        sourcetype = cfg_input['sourcetype']
        props_name = sourcetype
        transforms_metric_name = 'metric-schema:' + sourcetype
        transforms_perf_name = 'perfmonmk:' + sourcetype
        print("Converting: " + input_stanza)
        print("     Object: " + inputname)
        print("     SourceType: " + sourcetype)

        ##Create the props. If it exists, delete it.
        file_props_conf[sourcetype] = {}
        cfg_props = file_props_conf[sourcetype]
        cfg_props['INDEXED_EXTRACTIONS'] = 'tsv'
        cfg_props['LINE_BREAKER'] = '([\\r\\n]+)'        
        cfg_props['NO_BINARY_CHECK'] = '1'
        cfg_props['CATEGORY'] = 'Log To Metrics'
        cfg_props['PULLDOWN_TYPE'] = '1'
        cfg_props['METRIC-SCHEMA-TRANSFORMS'] = transforms_metric_name
        cfg_props['TRANSFORMS-perfmonMK'] = transforms_perf_name
        print("     Created Props.Conf > [" + props_name + "]")

        ##Now, for the "fun" part of building transforms.conf
        file_transforms_conf[transforms_metric_name] = {}
        cfg_trans_metric = file_transforms_conf[transforms_metric_name]
        cfg_trans_metric['METRIC-SCHEMA-MEASURES'] = '_ALLNUMS_'
        print("     Created Transforms.Conf > [" + transforms_metric_name + "]")
        #That was the easy one.

        file_transforms_conf[transforms_perf_name] = {}
        cfg_trans_perf = file_transforms_conf[transforms_perf_name]
        cfg_trans_perf['WRITE_META'] = '1'

        #Get a list of the "fields" expected on the _raw data.
        #raw_fields =  cfg_input['counters'].strip()

        input_counters_unmodified = cfg_input['counters'].split(';')
        input_counters_unmodified = list(filter(None, input_counters_unmodified)) 
        input_counters_modified = list(filter(None, input_counters_unmodified)) 
        totalitems = len(input_counters_modified)

        transform_perf_REGEX_Pre = r'collection=\"?([^\"\n]+)\"?\ncategory=\"?([^\"\n]+)\"?\nobject=\"?([^\"\n]+)\"?\n'
        transform_perf_FORMAT = f'collection::"$1" category::"$2" object::"$3" instance::"${totalitems + 5}" '
        transform_perf_REGEX = r"([^\t]+)\t"
        ##Strip out spaces, and trim the counter name.
        for idx, item in enumerate(input_counters_modified):            
            input_counters_modified[idx] = item.strip().replace(' ', '_')

        #Start building the wonderful regex!
        for idx, x in  enumerate(input_counters_unmodified):
            #print(f"{x}\t\t\t\t\t{input_counters_modified[idx]}")
            transform_perf_REGEX += r'([^\t]+)\t'
            transform_perf_FORMAT += f'"${5 + idx}"::"${totalitems + idx + 6}" '


        cfg_trans_perf['REGEX'] = transform_perf_REGEX_Pre + transform_perf_REGEX + r"\n" + transform_perf_REGEX + r"\n"
        cfg_trans_perf['FORMAT'] = transform_perf_FORMAT
        print("     Created Transforms.Conf > [" + transforms_perf_name + "]")

        



        












#Save the file
#with open('FILE.INI', 'w') as configfile:    # save
#    print('     Writing inputs.conf')
#    conf_inputs.write(configfile)



with open('props.conf', 'w') as f_props_conf:    # save
    print('     Writing props.conf...')
    file_props_conf.write(f_props_conf)


with open('transforms.conf', 'w') as f_trans_conf:    # save
    print('     Writing transforms.conf...')
    file_transforms_conf.write(f_trans_conf)