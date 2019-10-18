require "luasql.mysql"

print("connecting")

env = luasql.mysql()

conn = env:connect("dbname","user","pwd","IP address",3306)

print("connected")

conn:close() 
env:close()  