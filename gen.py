#really really quick script to process filelog 
#Will remove duplicate animations and change the offset, modify as needed


seen = []
newfile = ""
offset = 101162

with open("new_log.txt", "r") as f:

    for l in f:
        name = l.split(",")[0]
        if name not in seen:
            seen.append(name)
            try:
                line = l.split(",")
                line[2] = str( int ( line[2] ) - offset )
                line[3] = str( int ( line[3] ) - offset )

                newfile += ",".join(line)
            except:
                pass

with open("newer_log.txt","w") as f:
    f.write(newfile)