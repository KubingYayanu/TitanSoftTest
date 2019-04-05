const param = {
  countryInfo: `http://api.geonames.org/countryInfo`,
  search: `http://api.geonames.org/search`,
  username: "yayanu",
  selectCountryId: "country",
  selectCityId: "city",
  continentState: {},
  countriesData: undefined,
  citiesData: {}
}

const selectHelper = (function () {
  return {
    appendOption: function (name, text, value) {
      $(`#${name}`).append(
        $('<option>', {
          value: value,
          text: text
        })
      );
    },
    disableSelect: function (name, enabled) {
      $(`#${name}`).prop("disabled", enabled);
    },
    resetSelect: function (name) {
      $(`#${name}`)
        .find('option')
        .remove()
        .end()
        .append('<option>-- Please Select --</option>');
    }
  }
})();

const checkboxHelper = (function () {
  return {
    setChecked: function (name, value) {
      $(`input:checkbox[id=${name}]`).prop('checked', value);
    }
  }
})();

const ajaxHelper = (function () {
  return {
    getJson: function (url, data) {
      return $.get(url, data, "json");
    }
  }
})();

const arrayHelper = (function () {
  const min = function (m, n) {
    return (m < n) ? m : n;
  };

  const merge = function (data, left, middle, end, tmp, column) {
    let leftPtr = left;
    let rightPtr = middle;
    let j = left;
    while (j < end) {
      tmp[j++] = (leftPtr < middle &&
          (rightPtr >= end || data[leftPtr][column].toLowerCase() <= data[rightPtr][column].toLowerCase())) ?
        data[leftPtr++] :
        data[rightPtr++];
    }
    for (let i = left; i < end; i++)
      data[i] = tmp[i];
  };

  return {
    toJsonByColumn: function (array, column) {
      let newObj = {};
      $.each(array, function (key, value) {
        newObj[value[column]] = value;
      });
      return newObj;
    },
    mergeSort: function (data, column) {
      let tmp = [];
      const length = data.length;
      for (let width = 1; width < length; width = 2 * width) {
        for (let i = 0; i < length; i = i + 2 * width) {
          merge(data, i, min(i + width, length), min(i + 2 * width, length), tmp, column);
        }
      }
      return tmp;
    }
  }
})();

const countryService = (function () {
  const appendCountryData = function (jsonObj) {
    $.each(jsonObj, function (key, value) {
      selectHelper.appendOption(param.selectCountryId, value.countryName, value.countryCode);
    });
  };

  const loadData = function () {
    selectHelper.resetSelect(param.selectCountryId);

    const countriesData = param.countriesData;
    if (countriesData) {
      appendCountryData(countriesData);
    } else {
      const getData = {
        type: "json",
        username: param.username
      };

      selectHelper.disableSelect(param.selectCountryId, true);
      ajaxHelper.getJson(param.countryInfo, getData)
        .done(function (result) {
          const data = result.geonames;
          if (data) {
            const sortedData = arrayHelper.mergeSort(data, "countryName");
            const jsonObj = arrayHelper.toJsonByColumn(sortedData, "countryCode");
            param.countriesData = jsonObj;

            appendCountryData(jsonObj);
          }
        }).always(function () {
          selectHelper.disableSelect(param.selectCountryId, false);
        });
    }
  };

  const changeCountry = function () {
    $(`#${param.selectCountryId}`).change(function () {
      const countryCode = $(this).val();
      continentService.onRestoreContinent(countryCode);
      cityService.onLoadData(countryCode);
    });
  };

  return {
    init: function () {
      loadData();
      changeCountry();
    }
  }
})();

const cityService = (function () {
  const appendCityData = function (data) {
    $.each(data, function (key, value) {
      selectHelper.appendOption(param.selectCityId, value.name, value.name);
    });
  };

  return {
    onLoadData: function (countryCode) {
      selectHelper.resetSelect(param.selectCityId);

      const citiesData = param.citiesData[countryCode];
      if (citiesData) {
        appendCityData(citiesData);
      } else {
        const data = {
          type: "json",
          username: param.username,
          country: countryCode,
          featureCode: "PPLA2"
        };

        selectHelper.disableSelect(param.selectCityId, true);
        ajaxHelper.getJson(param.search, data)
          .done(function (result) {
            const data = result.geonames;
            if (data) {
              const sortedData = arrayHelper.mergeSort(data, "name");
              param.citiesData[countryCode] = sortedData;
              
              appendCityData(sortedData);
            }
          }).always(function () {
            selectHelper.disableSelect(param.selectCityId, false);
          });
      }
    }
  }
})();

const continentService = (function () {
  const continents = (function () {
    return $(":checkbox").map(function () {
      return $(this).attr('id');
    }).get();
  })();

  const setContinent = function () {
    $.each(continents, function (key, value) {
      $(`input:checkbox[id=${value}]`).change(function () {
        const countryCode = $(`#${param.selectCountryId}`).val();
        param.continentState[countryCode][value] = $(this).prop('checked');
      });
    });
  };

  const restoreContinent = function (countryCode) {
    const data = param.continentState[countryCode];

    if (data) {
      $.each(continents, function (key, value) {
        checkboxHelper.setChecked(value, data[value]);
      });
    } else {
      param.continentState[countryCode] = {};
      $.each(continents, function (key, value) {
        const continent = param.countriesData[countryCode].continent;
        const checked = continent === value ? true : false;

        param.continentState[countryCode][value] = checked;
        checkboxHelper.setChecked(value, checked);
      });
    }
  };

  return {
    init: function () {
      setContinent();
    },
    getCheckboxIds: function () {
      return continents;
    },
    onRestoreContinent: function (countryCode) {
      restoreContinent(countryCode);
    }
  }
})();