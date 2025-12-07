Here’s a consolidated markdown reference of **all OASA Telematics API endpoints and their responses** based on the public docs you linked. ([oasa-telematics-api.readthedocs.io][1])

You can copy-paste this directly into a `.md` file.

---

````markdown
# OASA Telematics API – Unified Endpoint Reference

Base URL (query-style API):

```text
http://telematics.oasa.gr/api/
````

All endpoints are called by passing:

* `act=<ActionName>`
* additional parameters as `p1=...&p2=...` or named query parameters (`line_code`, etc.)

In practice they are usually invoked as **HTTP POST** with parameters on the querystring, but they also work as plain query URLs.

---

## 1. getStopArrivals

Returns the next arriving vehicles for a given stop. ([oasa-telematics-api.readthedocs.io][2])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getStopArrivals&p1=stopcode
```

**Parameters**

* `p1` – `stopcode` (string): stop code (e.g. `400075`).

**Response**

Array of objects:

```json
[
  {
    "route_code": "2033",
    "veh_code": "50328",
    "btime2": "5"
  },
  {
    "route_code": "2005",
    "veh_code": "20521",
    "btime2": "5"
  }
]
```

Fields:

* `route_code`: route identifier.
* `veh_code`: vehicle identifier.
* `btime2`: minutes until arrival (stringified number).

---

## 2. getBusLocation

Returns locations of buses on a specific route. ([oasa-telematics-api.readthedocs.io][3])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getBusLocation&p1=routecode
```

**Parameters**

* `p1` – `routecode` (string): route code.

**Response**

Array of objects:

```json
[
  {
    "VEH_NO": "40860",
    "CS_DATE": "Jul 13 2016 11:05:32:000PM",
    "CS_LAT": "37.9018570",
    "CS_LNG": "23.7197450",
    "ROUTE_CODE": "2045"
  },
  {
    "VEH_NO": "40875",
    "CS_DATE": "Jul 13 2016 11:05:36:000PM",
    "CS_LAT": "37.9364170",
    "CS_LNG": "23.6405150",
    "ROUTE_CODE": "2045"
  }
]
```

Fields:

* `VEH_NO`: vehicle identifier.
* `CS_DATE`: timestamp of last position.
* `CS_LAT`, `CS_LNG`: latitude/longitude.
* `ROUTE_CODE`: route code.

---

## 3. getDailySchedule

Returns the daily (“come” and “go”) schedule for a line (start/end / timetable blocks). ([oasa-telematics-api.readthedocs.io][4])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getDailySchedule&line_code=lineCode
```

**Parameters**

* `line_code` – line code (e.g. `1079`).

**Response**

Object with `come` and `go` arrays:

```json
{
  "come": [
    {
      "line_id": "218",
      "sdd_code": "15743899",
      "sdc_code": "54",
      "sds_code": "3285",
      "sdd_aa": "2",
      "sdd_line1": null,
      "sdd_kp1": "0",
      "sdd_start1": null,
      "sde_start1": null,
      "sde_end1": null,
      "sdd_line2": "1035",
      "sdd_kp2": "0",
      "sde_start2": "1900-01-01 05:00:00",
      "sde_end2": "1900-01-01 05:45:00",
      "sdd_sort": "1",
      "line_circle": "0",
      "line_descr": "ΠΕΙΡΑΙΑΣ - ΣΤ. ΔΑΦΝΗ",
      "line_descr_eng": "PEIRAIAS - ST. DAFNI",
      "remarks": null
    }
    /* ... */
  ],
  "go": [
    {
      "line_id": "218",
      "sdd_code": "15743895",
      "sdc_code": "54",
      "sds_code": "3284",
      "sdd_aa": "2",
      "sdd_line1": "1035",
      "sdd_kp1": "0",
      "sdd_start1": "Jan 1 1900 05:00:00:000AM",
      "sde_start1": "1900-01-01 05:00:00",
      "sde_end1": "1900-01-01 05:50:00",
      "sdd_line2": "1035",
      "sdd_kp2": "0",
      "sde_start2": "1900-01-01 05:55:00",
      "sde_end2": "1900-01-01 06:44:00",
      "sdd_sort": "1",
      "line_circle": "0",
      "line_descr": "ΠΕΙΡΑΙΑΣ - ΣΤ. ΔΑΦΝΗ",
      "line_descr_eng": "PEIRAIAS - ST. DAFNI",
      "remarks": null
    }
    /* ... */
  ]
}
```

---

## 4. getEsavlDirections (NOT really implemented)

Used internally for “optimal route” – currently returns `null` according to docs. ([oasa-telematics-api.readthedocs.io][5])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getEsavlDirections&p1=4&p2=3&p3=300&p4=from[1]&p5=from[0]&p6=to[1]&p7=to[0]
```

**Parameters (guessed by author)**

* `from[0]`, `from[1]` – latitude/longitude of origin.
* `to[0]`, `to[1]` – latitude/longitude of destination.
* Others (`p1`, `p2`, `p3`) are opaque.

**Response**

```json
null
```

---

## 5. getScheduleDaysMasterline

Returns schedule “day types” (e.g. SUMMER DAILY / SATURDAY / SUNDAY) for a masterline. ([oasa-telematics-api.readthedocs.io][6])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getScheduleDaysMasterline&p1=linecode
```

**Parameters**

* `p1` – `linecode`: line code.

**Response**

Array of objects:

```json
[
  {
    "sdc_descr": "ΘΕΡΙΝΟ 1 ΚΑΘΗΜΕΡΙΝΗ",
    "sdc_descr_eng": "SUMMER DAILY",
    "sdc_code": "86",
    "": "0"
  },
  {
    "sdc_descr": "ΘΕΡΙΝΟ 1 ΣΑΒΒΑΤΟ",
    "sdc_descr_eng": "SUMMER SATURDAY",
    "sdc_code": "87",
    "": "1"
  },
  {
    "sdc_descr": "ΘΕΡΙΝΟ 1 ΚΥΡΙΑΚΗ",
    "sdc_descr_eng": "SUMMER SUNDAY",
    "sdc_code": "88",
    "": "0"
  }
]
```

Key field:

* `sdc_code`: schedule program code (needed for `getSchedLines`).

---

## 6. getLinesAndRoutesForMl

Returns lines/routes belonging to a specific master line (`mlcode`). ([oasa-telematics-api.readthedocs.io][7])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getLinesAndRoutesForMl&p1=mlcode
```

**Parameters**

* `p1` – `mlcode`: master line code (from `webGetLinesWithMLInfo` / `webGetMasterLines`).

**Response**

Array of objects like:

```json
[
  {
    "afetiria": "ΑΝΩ ΝΕΑ ΣΜΥΡΝΗ ",
    "afetiria_en": "ANO NEA SMIRNI",
    "line_code": "1030",
    "line_descr": "ΑΝΩ Ν. ΣΜΥΡΝΗ Α - ΣΤ. ΣΥΓΓΡΟΥ ΦΙΞ (ΚΥΚΛΙΚΗ)",
    "line_descr_eng": "ANO N. SMYRNI A - ST. SYGGROY FIX ",
    "line_id": "137",
    "line_id_gr": "137",
    "sdc_code": "97",
    "terma": "ΑΝΩ ΝΕΑ ΣΜΥΡΝΗ ",
    "terma_en": "ANO NEA SMIRNI"
  }
  /* ... */
]
```

---

## 7. getRoutesForLine

Like `webGetRoutes` but under the “get” namespace. Returns routes for a given line code. ([oasa-telematics-api.readthedocs.io][8])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getRoutesForLine&p1=linecode
```

**Parameters**

* `p1` – `linecode`: line code.

**Response**

Array of route objects:

```json
[
  {
    "route_code": "2045",
    "route_id": "01",
    "route_descr": "ΠΕΙΡΑΙΑΣ - ΒΟΥΛΑ",
    "route_active": "1",
    "route_descr_eng": "PEIRAIAS - VOULA"
  },
  {
    "route_code": "2046",
    "route_id": "02",
    "route_descr": "ΒΟΥΛΑ - ΠΕΙΡΑΙΑΣ",
    "route_active": "1",
    "route_descr_eng": "VOULA - PEIRAIAS"
  }
]
```

---

## 8. getMLName

Returns description of a masterline by `mlcode`. ([oasa-telematics-api.readthedocs.io][9])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getMLName&p1=mlcode
```

**Parameters**

* `p1` – `mlcode`.

**Response**

```json
[
  {
    "ml_descr": "ΠΕΙΡΑΙΑΣ - ΒΟΥΛΑ",
    "ml_descr_eng": null
  }
]
```

---

## 9. getLineName

Returns line name by `linecode`. ([oasa-telematics-api.readthedocs.io][10])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getLineName&p1=linecode
```

**Parameters**

* `p1` – `linecode`.

**Response**

```json
[
  {
    "line_descr": "ΠΕΙΡΑΙΑΣ - ΒΟΥΛΑ",
    "line_descr_eng": "PEIRAIAS - VOYLA"
  }
]
```

---

## 10. getLinesAndRoutesForMlandLCode (NOT implemented)

Marked as “Not Implemented probably”; returns `null`. ([oasa-telematics-api.readthedocs.io][11])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getLinesAndRoutesForMl&p1=mlcode&p2=lid
```

**Response**

```json
null
```

---

## 11. getPlaces (NOT implemented)

Returns `null` according to docs. ([oasa-telematics-api.readthedocs.io][12])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getPlaces&p1=catCode
```

**Parameters**

* `p1` – `catCode`: category code.

**Response**

```json
null
```

---

## 12. getPlacesTypes (NOT implemented)

Also returns `null`. ([oasa-telematics-api.readthedocs.io][13])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getPlacesTypes
```

**Response**

```json
null
```

---

## 13. getRouteName

Returns route name from `routecode`. ([oasa-telematics-api.readthedocs.io][14])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getRouteName&p1=routecode
```

**Parameters**

* `p1` – `routecode`.

**Response**

```json
[
  {
    "route_descr": "ΠΕΙΡΑΙΑΣ - ΒΟΥΛΑ",
    "route_departure_eng": "PEIRAIAS - VOULA"
  }
]
```

---

## 14. getStopNameAndXY

Returns stop name + coordinates for a `stopcode`. ([oasa-telematics-api.readthedocs.io][15])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getStopNameAndXY&p1=stopcode
```

**Parameters**

* `p1` – `stopcode`.

**Response**

```json
[
  {
    "stop_descr": "ΗΣΑΠ Ν.ΦΑΛΗΡΟΥ",
    "stop_descr_matrix_eng": "ISAP.N.FALIROY",
    "stop_lat": "37.9445913",
    "stop_lng": "23.6671421",
    "stop_heading": "88",
    "stop_id": "400075"
  }
]
```

---

## 15. getSchedLines

More detailed schedule times for a combination of masterline, schedule code and line code. ([oasa-telematics-api.readthedocs.io][16])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getSchedLines&p1=mlCode&p2=sdcCode&p3=lineCode
```

**Parameters**

* `p1` – `mlCode`: master line code.
* `p2` – `sdcCode`: schedule-day code (from `getScheduleDaysMasterline`).
* `p3` – `lineCode`: line code.

**Response**

Object with `come` and `go` arrays, similar to:

```json
{
  "come": [
    {
      "line_id": "11",
      "sde_code": "816784",
      "sdc_code": "93",
      "sds_code": "1292",
      "sde_aa": "7",
      "sde_line1": "1079",
      "sde_kp1": "0",
      "sde_start1": "1900-01-01 23:15:00",
      "sde_end1": "1900-01-02 00:00:00",
      "sde_line2": "1079",
      "sde_kp2": "0",
      "sde_start2": "1900-01-02 00:00:00",
      "sde_end2": "1900-01-02 00:45:00",
      "sde_sort": "980",
      "sde_descr2": null,
      "line_circle": "0",
      "line_descr": "ΑΝΩ ΠΑΤΗΣΙΑ - Ν. ΠΑΓΚΡΑΤΙ - Ν. ΕΛΒΕΤΙΑ",
      "line_descr_eng": "ANO PATISIA - N. PAGKRATI - N. ELVETIA"
    }
    /* ... */
  ],
  "go": [
    {
      "line_id": "11",
      "sde_code": "816763",
      "sdc_code": "93",
      "sds_code": "1290",
      "sde_aa": "13",
      "sde_line1": "1079",
      "sde_kp1": "0",
      "sde_start1": "1900-01-02 00:35:00",
      "sde_end1": "1900-01-02 01:20:00",
      "sde_line2": "1079",
      "sde_kp2": "0",
      "sde_start2": "1900-01-02 01:20:00",
      "sde_end2": "1900-01-02 02:05:00",
      "sde_sort": "1",
      "sde_descr1": null,
      "line_circle": "0",
      "line_descr": "ΑΝΩ ΠΑΤΗΣΙΑ - Ν. ΠΑΓΚΡΑΤΙ - Ν. ΕΛΒΕΤΙΑ",
      "line_descr_eng": "ANO PATISIA - N. PAGKRATI - N. ELVETIA"
    }
    /* ... */
  ]
}
```

---

## 16. getClosestStops

Returns stops closest to coordinates (`x`, `y`). ([oasa-telematics-api.readthedocs.io][17])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=getClosestStops&p1=x&p2=y
```

**Parameters**

* `p1` – `x`: longitude.
* `p2` – `y`: latitude.

**Response**

Array of stops:

```json
[
  {
    "StopCode": "400058",
    "StopID": "400058",
    "StopDescr": "ΒΕΝΙΖΕΛΟΥ",
    "StopDescrEng": "VENIZELOY",
    "StopStreet": "ΓΡ.ΛΑΜΠΡΑΚΗ",
    "StopStreetEng": null,
    "StopHeading": "42",
    "StopLat": "37.9432677",
    "StopLng": "23.6520113",
    "distance": "0.001443313361679744"
  }
  /* ... */
]
```

---

## 17. webGetLines

Returns info about **all bus lines**. ([oasa-telematics-api.readthedocs.io][18])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetLines
```

**Response**

Array:

```json
[
  {
    "LineCode": "815",
    "LineID": "021",
    "LineDescr": "ΠΛΑΤΕΙΑ ΚΑΝΙΓΓΟΣ - ΓΚΥΖΗ",
    "LineDescrEng": "PLATEIA KANIGKOS - GKIZI"
  }
  /* ... */
]
```

Field meanings (from docs):

* `LineCode`: unique line code (internal primary key, also used as URI).
* `LineID`: human-visible line number (string).
* `LineDescr`: Greek title (Unicode).
* `LineDescrEng`: English/ASCII title.

---

## 18. webGetLinesWithMLInfo

Same as `webGetLines` but with masterline and schedule info. ([oasa-telematics-api.readthedocs.io][19])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetLinesWithMLInfo
```

**Response**

Array:

```json
[
  {
    "ml_code": "9",
    "sdc_code": "86",
    "line_code": "815",
    "line_id": "021",
    "line_descr": "ΠΛΑΤΕΙΑ ΚΑΝΙΓΓΟΣ - ΓΚΥΖΗ",
    "line_descr_eng": "PLATEIA KANIGKOS - GKIZI"
  }
  /* ... */
]
```

Extra fields:

* `ml_code`: masterline code.
* `sdc_code`: schedule code (used in schedule endpoints).

---

## 19. webGetRoutes

Returns routes for a given line. ([oasa-telematics-api.readthedocs.io][20])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetRoutes&p1=linecode
```

**Parameters**

* `p1` – `linecode`.

**Response**

```json
[
  {
    "RouteCode": "2045",
    "LineCode": "962",
    "RouteDescr": "ΠΕΙΡΑΙΑΣ - ΒΟΥΛΑ",
    "RouteDescrEng": "PEIRAIAS - VOULA",
    "RouteType": "1",
    "RouteDistance": "22385.44"
  },
  {
    "RouteCode": "2046",
    "LineCode": "962",
    "RouteDescr": "ΒΟΥΛΑ - ΠΕΙΡΑΙΑΣ",
    "RouteDescrEng": "VOULA - PEIRAIAS",
    "RouteType": "2",
    "RouteDistance": "22644.61"
  }
]
```

---

## 20. webGetStops

Returns stops for a specific route. ([oasa-telematics-api.readthedocs.io][21])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetStops&p1=routecode
```

**Parameters**

* `p1` – `routecode`.

**Response**

```json
[
  {
    "StopCode": "10183",
    "StopID": "25",
    "StopDescr": "ΠΕΙΡΑΙΑΣ",
    "StopDescrEng": "PEIRAIAS",
    "StopStreet": null,
    "StopStreetEng": null,
    "StopHeading": "93",
    "StopLat": "37.938246",
    "StopLng": "23.6320605",
    "RouteStopOrder": "1",
    "StopType": "0",
    "StopAmea": "0"
  }
  /* ... */
]
```

---

## 21. webRouteDetails

Geometric path of a route (sequence of points). ([oasa-telematics-api.readthedocs.io][22])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webRouteDetails&p1=routecode
```

**Parameters**

* `p1` – `routecode`.

**Response**

Array of points:

```json
[
  { "routed_x": "23.63272", "routed_y": "37.93851", "routed_order": "1" },
  { "routed_x": "23.6326",  "routed_y": "37.93851", "routed_order": "2" },
  { "routed_x": "23.63233", "routed_y": "37.93833", "routed_order": "3" }
  /* ... */
]
```

---

## 22. webRoutesForStop

Routes that serve a given stop. ([oasa-telematics-api.readthedocs.io][23])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webRoutesForStop&p1=stopcode
```

**Parameters**

* `p1` – `stopcode`.

**Response**

```json
[
  {
    "RouteCode": "1867",
    "LineCode": "851",
    "RouteDescr": "ΠΕΙΡΑΙΑΣ - Ν. ΣΜΥΡΝΗ",
    "RouteDescrEng": "PEIRAIAS - NEA SMYRNI",
    "RouteType": "1",
    "RouteDistance": "22205.03",
    "LineID": "130",
    "LineDescr": "ΠΕΙΡΑΙΑΣ - Ν. ΣΜΥΡΝΗ (ΚΥΚΛΙΚΗ)",
    "LineDescrEng": "PEIRAIAS - NEA SMIRNI",
    "MasterLineCode": "202"
  }
  /* ... */
]
```

---

## 23. webGetRoutesDetailsAndStops

Combined route geometry + stop list in one call. ([oasa-telematics-api.readthedocs.io][24])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetRoutesDetailsAndStops&p1=routecode
```

**Parameters**

* `p1` – `routecode`.

**Response**

```json
{
  "details": [
    { "routed_x": "23.63272", "routed_y": "37.93851", "routed_order": "1" }
    /* ... */
  ],
  "stops": [
    {
      "StopCode": "10183",
      "StopID": "25",
      "StopDescr": "ΠΕΙΡΑΙΑΣ",
      "StopDescrEng": "PEIRAIAS",
      "StopStreet": null,
      "StopStreetEng": null,
      "StopHeading": "93",
      "StopLat": "37.938246",
      "StopLng": "23.6320605",
      "RouteStopOrder": "1",
      "StopType": "0",
      "StopAmea": "0"
    }
    /* ... */
  ]
}
```

---

## 24. webGetLangs

Localization strings used by the OASA web UI. ([oasa-telematics-api.readthedocs.io][25])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetLangs
```

**Response**

```json
[
  {
    "lang_id": "1",
    "el": "Πληροφορίες Γραμμής",
    "en": "Line Information"
  }
  /* ... */
]
```

Fields:

* `lang_id`: string id.
* `el`: Greek text (Unicode).
* `en`: English text (ASCII).

---

## 25. webGetMasterLines

Masterlines and related info. ([oasa-telematics-api.readthedocs.io][26])

**Endpoint**

```text
http://telematics.oasa.gr/api/?act=webGetMasterLines
```

**Response**

```json
[
  {
    "ml_code": "9",
    "ml_descr": "ΚΑΝΙΓΓΟΣ - ΓΚΥΖΗ",
    "ml_descr_eng": null,
    "ml_id": "021",
    "sdc_code": "54",
    "line_code": "1151",
    "is_complex": "2"
  }
  /* ... */
]
```

Field hints (from docs):

* `ml_id`: probably the human line number.
* `ml_code`: masterline identifier.
* `sdc_code`: schedule code (see schedule APIs).
* `line_code`: associated line code.
* `ml_descr`: Greek description.
* `ml_descr_eng`: currently `null` in examples.
* `is_complex`: unknown meaning.

---

## Notes / Practical Tips

* **Encoding:** Greek strings are UTF-8; English ones are ASCII-friendly.
* **Types:** Nearly everything comes as **strings** (even numeric values, distances, and times). Cast on the client.
* **Time fields:** Many schedule times are relative dates like `1900-01-01` – they encode time of day, not real calendar dates.
* **Unimplemented endpoints:** `getEsavlDirections`, `getPlaces`, `getPlacesTypes`, `getLinesAndRoutesForMlandLCode` are effectively `null` in the public API right now.

```

---

If you want, next step I can turn this into a more “code-friendly” spec (e.g. OpenAPI-ish YAML or C# DTO list) so you can just feed it straight to Copilot and generate clients.
::contentReference[oaicite:26]{index=26}
```

[1]: https://oasa-telematics-api.readthedocs.io/en/latest/ "Welcome to OASA Telematics API’s documentation! — OASA Telematics API 0.1 documentation"
[2]: https://oasa-telematics-api.readthedocs.io/en/latest/getStopArrivals.html "getStopArrivals — OASA Telematics API 0.1 documentation"
[3]: https://oasa-telematics-api.readthedocs.io/en/latest/getBusLocation.html "getBusLocation — OASA Telematics API 0.1 documentation"
[4]: https://oasa-telematics-api.readthedocs.io/en/latest/getDailySchedule.html "getDailySchedule — OASA Telematics API 0.1 documentation"
[5]: https://oasa-telematics-api.readthedocs.io/en/latest/getEsavlDirections.html "getEsavlDirections — OASA Telematics API 0.1 documentation"
[6]: https://oasa-telematics-api.readthedocs.io/en/latest/getScheduleDaysMasterline.html "getScheduleDaysMasterline — OASA Telematics API 0.1 documentation"
[7]: https://oasa-telematics-api.readthedocs.io/en/latest/getLinesAndRoutesForMl.html "getLinesAndRoutesForMl — OASA Telematics API 0.1 documentation"
[8]: https://oasa-telematics-api.readthedocs.io/en/latest/getRoutesForLine.html "getRoutesForLine — OASA Telematics API 0.1 documentation"
[9]: https://oasa-telematics-api.readthedocs.io/en/latest/getMLName.html "getMLName — OASA Telematics API 0.1 documentation"
[10]: https://oasa-telematics-api.readthedocs.io/en/latest/getLineName.html "getLineName — OASA Telematics API 0.1 documentation"
[11]: https://oasa-telematics-api.readthedocs.io/en/latest/getLinesAndRoutesForMlandLCode.html "getLinesAndRoutesForMlandLCode — OASA Telematics API 0.1 documentation"
[12]: https://oasa-telematics-api.readthedocs.io/en/latest/getPlaces.html "getPlaces — OASA Telematics API 0.1 documentation"
[13]: https://oasa-telematics-api.readthedocs.io/en/latest/getPlacesTypes.html "getPlacesTypes — OASA Telematics API 0.1 documentation"
[14]: https://oasa-telematics-api.readthedocs.io/en/latest/getRouteName.html "getRouteName — OASA Telematics API 0.1 documentation"
[15]: https://oasa-telematics-api.readthedocs.io/en/latest/getStopNameAndXY.html "getStopNameAndXY — OASA Telematics API 0.1 documentation"
[16]: https://oasa-telematics-api.readthedocs.io/en/latest/getSchedLines.html "getSchedLines — OASA Telematics API 0.1 documentation"
[17]: https://oasa-telematics-api.readthedocs.io/en/latest/getClosestStops.html "getClosestStops — OASA Telematics API 0.1 documentation"
[18]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetLines.html "webGetLines — OASA Telematics API 0.1 documentation"
[19]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetLinesWithMLInfo.html "webGetLinesWithMLInfo — OASA Telematics API 0.1 documentation"
[20]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetRoutes.html "webGetRoutes — OASA Telematics API 0.1 documentation"
[21]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetStops.html "webGetStops — OASA Telematics API 0.1 documentation"
[22]: https://oasa-telematics-api.readthedocs.io/en/latest/webRouteDetails.html "webRouteDetails — OASA Telematics API 0.1 documentation"
[23]: https://oasa-telematics-api.readthedocs.io/en/latest/webRoutesForStop.html "webRoutesForStop — OASA Telematics API 0.1 documentation"
[24]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetRoutesDetailsAndStops.html "webGetRoutesDetailsAndStops — OASA Telematics API 0.1 documentation"
[25]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetLangs.html "webGetLangs — OASA Telematics API 0.1 documentation"
[26]: https://oasa-telematics-api.readthedocs.io/en/latest/webGetMasterLines.html "webGetMasterLines — OASA Telematics API 0.1 documentation"
