mergeInto(LibraryManager.library, {
	BuildWeb: function () {
		window.alert("Hello, Web!");
	},
	Show: function (str, type) {
		VueShow(UTF8ToString(str), UTF8ToString(type));
	},
});