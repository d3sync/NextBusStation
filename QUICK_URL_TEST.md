# Quick Test - Copy/Paste These URLs

Test these **exact URLs** in your browser to verify which parameter order works:

## Test 1: Current Order (lon, lat)
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=23.7348&p2=37.9755
```
**This is what the app uses**

## Test 2: Swapped Order (lat, lon)
```
http://telematics.oasa.gr/api/?act=getClosestStops&p1=37.9755&p2=23.7348
```
**Just in case the docs are wrong**

## Expected Results

**One of them should return JSON like**:
```json
[
  {
    "StopCode":"010001",
    "StopID":"10001",
    "StopDescr":"сумтацла",
    "StopDescrEng":"SYNTAGMA",
    ...
  }
]
```

**The wrong one will return**:
```json
[]
```

---

## What to Do

1. Open browser
2. Paste **Test 1** URL
3. If you get stops ? **Current code is correct** ?
4. If you get `[]` ? Try **Test 2** URL
5. If Test 2 works ? **I'll swap the parameters**
6. If both return `[]` ? **OASA API might be down**

---

**Reply with which one works!** ??
