/*
Copyright (c) 2016 libertylocked

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

package controlstore

import (
	"encoding/json"
	"sync"
)

// CacheStore represents a collection of KV stores, one for each service
type CacheStore struct {
	stores map[byte]*KVStore
	mutex  sync.RWMutex
}

// GetStore gets the KV cache store for the service
// Returns nil if store does not exist
func (c *CacheStore) GetStore(svcName byte) *KVStore {
	c.mutex.RLock()
	kvstore := c.stores[svcName]
	c.mutex.RUnlock()

	return kvstore
}

// CreateStore creates a KV store for a service. Overwrites the old store if exists
// Returns the created store
func (c *CacheStore) CreateStore(svcName byte) *KVStore {
	kvstore := NewKVStore()
	c.mutex.Lock()
	c.stores[svcName] = kvstore
	c.mutex.Unlock()

	return kvstore
}

// DeleteStore deletes the KV store for a service. Does nothing if store does not exist
func (c *CacheStore) DeleteStore(svcName byte) {
	c.mutex.Lock()
	delete(c.stores, svcName)
	c.mutex.Unlock()
}

// SetCache sets a KV pair in a service's KV store. Creates the store if not exists
func (c *CacheStore) SetCache(svcName byte, key byte, value byte) {
	svcStore := c.GetStore(svcName)
	if svcStore == nil {
		svcStore = c.CreateStore(svcName)
	}

	svcStore.Set(key, value)
}

// GetCache gets the value for a given key in a service's KV store
func (c *CacheStore) GetCache(svcName byte, key byte) byte {
	svcStore := c.GetStore(svcName)
	if svcStore == nil {
		return 0x00
	}
	return svcStore.Get(key)
}

// DeleteCache deletes a KV pair in a service's KV store
func (c *CacheStore) DeleteCache(svcName byte, key byte) {
	svcStore := c.GetStore(svcName)
	if svcStore != nil {
		svcStore.Delete(key)
	}
}

// MarshalJSON marshals a CacheStore as a JSON object
func (c *CacheStore) MarshalJSON() ([]byte, error) {
	c.mutex.RLock()
	bytes, err := json.Marshal(&c.stores)
	c.mutex.RUnlock()
	return bytes, err
}

// NewCacheStore creates a new CacheStore
func NewCacheStore() *CacheStore {
	return &CacheStore{map[byte]*KVStore{}, sync.RWMutex{}}
}
