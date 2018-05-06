var typeKeywordDictionary = new Map();
typeKeywordDictionary["integer"] = "int";
typeKeywordDictionary["number"] = "double";
typeKeywordDictionary["text"] = "string";

var codeParserDictionary = new Map();
codeParserDictionary.set(/([A-Za-z]+) (am|are|is) (.+)\./, (match) => {
    var name = match[1];
    var value = match[3];
    
    return `var ${name} = ${value};`;
});

function parse(code) {
    for (var [regex, func] of codeParserDictionary) {
        if (regex.test(code)) {
            var match = regex.exec(code);
            return func(match);
        }
    }

    return code;
}