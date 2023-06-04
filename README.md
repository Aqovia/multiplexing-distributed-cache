# Aqovia Multiplexing Distributed Cache

This repository contains code for Multiplexing Distributed Cache.

1. Multiplexing Distributed Cache is an implementation of IDistributedCache that is multiplexing to two other implementation of IDistributedCache.
2. The write is to both, but read will be always from the Primary cache that you can specify when setting it up.
