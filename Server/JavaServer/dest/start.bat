title JiaoYouAdvServer
java -Xmx3000m -Xms3000m -Xmn2g -XX:NewRatio=4 -XX:SurvivorRatio=4 -XX:MaxTenuringThreshold=0 -XX:+UseParallelGC -XX:ParallelGCThreads=4 -jar Server.jar
pause