Vue.component('suggestions', {
  template: '#suggestions_template',
  props: ['message', 'suggestions'],
  data() {
    return {};
  },
  computed: {
    currentSuggestions() {
      const normalizedMessage = this.message.toLowerCase(); // Convert message to lowercase

      if (normalizedMessage === '') {
        return [];
      }

      const currentSuggestions = this.suggestions.filter((s) => {
        const normalizedSuggestionName = s.name.toLowerCase(); // Convert suggestion name to lowercase

        if (!normalizedSuggestionName.startsWith(normalizedMessage)) {
          const suggestionSplitted = normalizedSuggestionName.split(' ');
          const messageSplitted = normalizedMessage.split(' ');

          for (let i = 0; i < messageSplitted.length; i += 1) {
            if (i >= suggestionSplitted.length) {
              return i < suggestionSplitted.length + s.params.length;
            }
            if (suggestionSplitted[i] !== messageSplitted[i]) {
              return false;
            }
          }
        }
        return true;
      }).slice(0, CONFIG.suggestionLimit);

      currentSuggestions.forEach((s) => {
        s.disabled = !s.name.startsWith(normalizedMessage);

        s.params.forEach((p, index) => {
          const wType = (index === s.params.length - 1) ? '.' : '\\S';
          const regex = new RegExp(`${s.name} (?:\\w+ ){${index}}(?:${wType}*)$`, 'g');

          p.disabled = normalizedMessage.match(regex) == null;
        });
      });

      return currentSuggestions;
    },
  },
  methods: {},
});